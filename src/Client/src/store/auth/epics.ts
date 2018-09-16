import { Observable, from, of } from 'rxjs';
import { mergeMap, map, filter, withLatestFrom, catchError } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import { authActions, AuthAction, Failure, GeneralFailure, generalFailure, AuthState, validationFailure, Invalid } from '.';
import { isActionOf, action } from 'typesafe-actions';
import { RootState } from '../store';
import { loginOrCreateCompleted, loginOrCreateErrored } from './actions';
import { hash } from "bcryptjs";
import { lowerFirst } from "lodash";
import * as t from 'io-ts';
import { PathReporter } from 'io-ts/lib/PathReporter';
import either, { Either } from 'fp-ts/lib/Either';
import taskEither, { TaskEither } from 'fp-ts/lib/TaskEither';
import option, { Option } from 'fp-ts/lib/Option';
import { Task } from 'fp-ts/lib/Task';
import { Unit } from '../shared/model';
import { identity } from 'fp-ts/lib/function';

export function getVault(action: Observable<AuthAction>, state: Observable<RootState>) {
    return action.pipe(
        filter(isActionOf(authActions.loginOrCreateInitiated)),
        withLatestFrom(state),
        map(([_, { auth }]) => auth),
        mergeMap(auth => {
            from()

        })ajax.getJSON(`/api/vault`, toHeaders(auth.username!, hashedPassword))
    mergeMap(auth =>
            from(toHash(auth.username!, auth.password!))
                .pipe(
                    map(hashedPassword => ajax.getJSON(`/api/vault`, toHeaders(auth.username!, hashedPassword))),
                    map(response => loginOrCreateCompleted()),
                    catchError(error => {
                        console.log("Something failed", error)
                        return of(loginOrCreateCompleted());
                    })
                )
        ));
}



const GetSaltSuccessResponse = t.type({
    salt: t.union([t.null, t.string])
});

const GetSaltValidationErrorResponse = t.type({
    validationErrors: t.type({
        username: t.array(t.string)
    })
});

const GetSaltErrorResponse = t.type({
    message: t.string
});

export class Salt {
    constructor(public readonly value: string) { }
}

export type SaltError = GeneralFailure | Invalid<AuthState> | Unit;

function getSalt(username: string): TaskEither<SaltError, Option<Salt>> {
    const actionDescription = "Logging you in";
    return taskEither.tryCatch(
        () => fetch(`/api/salt`, { headers: toUsernameAuthHeaders(username) }),
        (error: any) => mapFetchError(error, actionDescription))
        .mapLeft<SaltError>(identity)
        .chain(response => {
            const contentType = response.headers.get("content-type");
            if (contentType === "application/json") {
                if (response.status == 200) {
                    return parseJsonResponse(response, GetSaltSuccessResponse, actionDescription)
                        .map(({ salt }) => salt != null ? option.some(new Salt(salt)) : option.none);
                } else if (response.status == 400) {
                    return parseJsonResponse(response, GetSaltValidationErrorResponse, actionDescription)
                        .mapLeft<SaltError>(identity)
                        .chain(({ validationErrors }) =>
                            taskEither.fromLeft(validationFailure<AuthState>({ errors: validationErrors })));
                } else if (response.status == 500) {
                    return parseJsonResponse(response, GetSaltErrorResponse, actionDescription)
                        .chain(({ message }) => taskEither.fromLeft(generalFailure({
                            friendly: { actionDescription: actionDescription, reason: message },
                            error: { responseCode: 500, message }
                        })));
                }
            }

            return parseTextResponse(response, actionDescription)
                .chain(text => taskEither.fromLeft(Errors.Internet.unexpectedResponse(actionDescription, {
                    message: `Response with unexpected status code ${response.status} and content-type ${contentType}`,
                    response: text
                })));
        });
}

export interface ResponseType<T, L, R> {
    match: (response: Response) => boolean;
    validator: t.Type<T>;
    chain: (response: T) => TaskEither<L, R>;
}

function fetchJson<T1, L1, R1, T2, L2, R2, T3, L3, R3>(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: [ResponseType<T1, L1, R1>, ResponseType<T2, L2, R2>, ResponseType<T3, L3, R3>]): TaskEither<GeneralFailure | L1 | L2 | L3, R1 | R2 | R3>;
function fetchJson<T1, L1, R1, T2, L2, R2>(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: [ResponseType<T1, L1, R1>, ResponseType<T2, L2, R2>]): TaskEither<GeneralFailure | L1 | L2, R1 | R2>;
function fetchJson<T1, L1, R1>(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: [ResponseType<T1, L1, R1>]): TaskEither<GeneralFailure | L1, R1>;
function fetchJson(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: ResponseType<any, any, any>[]) {
    return taskEither.tryCatch(
        () => fetch(input, init),
        (error: any) => mapFetchError(error, actionDescription))
        .mapLeft<SaltError>(identity)
        .chain(response => {
            const contentType = response.headers.get("content-type");
            if (contentType === "application/json") {
                const matchingResponseType = responseTypes.find(responseType => responseType.match(response));
                if (matchingResponseType !== undefined) {
                    return parseJsonResponse(response, matchingResponseType.validator, actionDescription)
                        .chain(matchingResponseType.chain);
                }
            }

            return parseTextResponse(response, actionDescription)
                .chain(body => taskEither.fromLeft(Errors.Internet.unexpectedResponse(actionDescription, {
                    message: `Response with unexpected status code '${response.status}' and content-type '${contentType}'`,
                    body: body
                })));
        });
}

function parseTextResponse(response: Response, actionDescription: string): TaskEither<GeneralFailure, string> {
    return taskEither.tryCatch(
        response.text,
        (error: any) =>
            Errors.Internet.unexpectedResponse(actionDescription, {
                message: "Calling response.text returned an error",
                error
            }));
}


function parseJsonResponse<T>(response: Response, validator: t.Type<T>, actionDescription: string): TaskEither<GeneralFailure, T> {
    return taskEither.tryCatch(
        response.json,
        (error: any) =>
            Errors.Internet.unexpectedResponse(actionDescription, {
                message: "Calling response.json returned an error",
                error
            }))
        .chain(data =>
            taskEither.fromEither(parse(data, validator, actionDescription)));
}

function parse<T>(data: any, validator: t.Type<T>, actionDescription: string): Either<GeneralFailure, T> {
    return validator
        .decode(data)
        .mapLeft(validationErrors => {
            const errors = PathReporter.report(either.left(validationErrors))
            return Errors.Internet.unexpectedResponse(actionDescription, {
                message: "Failed to parse expected type " + validator.name,
                parseErrors: errors, data: data
            });
        });
}

function mapFetchError(error: unknown, actionDescription: string): GeneralFailure | Unit {
    if (error instanceof TypeError) {
        return Errors.Internet.connectionFailed(actionDescription, error);
    } else if (error instanceof DOMException && error.name === "AbortError") {
        return [];
    } else {
        return Errors.Internet.unknown(actionDescription, error);
    }
}

export module Errors {
    export module Internet {

        export function unexpectedResponse(actionDescription: string, error: unknown) {
            return generalFailure({
                friendly: {
                    actionDescription: actionDescription,
                    reason: "Our server returned an unexpected response or someone else's server responded"
                },
                error: error
            });
        }

        export function connectionFailed(actionDescription: string, error: unknown) {
            return generalFailure({
                friendly: {
                    actionDescription: actionDescription,
                    reason: "There is something wrong with your internet connection or our server is down"
                },
                error: error,
                possibleSolutions: [
                    "Are you connected to the internet?",
                    "If our server is down, then there's not much you can do but keep trying until we come back online."
                ]
            });
        }

        export function unknown(actionDescription: string, error: unknown) {
            return generalFailure({
                friendly: {
                    actionDescription: actionDescription,
                    reason: "An unknown error occurred while trying to connect to our server."
                },
                error: error,
                possibleSolutions: [
                    "Are you connected to the internet?",
                    "If our server is down, then there's not much you can do but keep trying until we come back online."
                ]
            });
        }
    }
}

function toHash(username: string, password: string): Promise<string> {
    return hash(password, username);
}

function toUsernameAuthHeaders(username: string) {
    return toAuthHeader({ username });
}

type HashedPassword = string;

function toAuthHeaders(username: string, password: HashedPassword) {
    return toAuthHeader({ username, password });
}

function toAuthHeader(obj: any) {
    return { "Authorization": `Basic ${toBase64(obj)}` };
}

function toBase64(obj: any) {
    return btoa(JSON.stringify(obj))
}