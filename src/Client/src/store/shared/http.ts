import { GeneralFailure, generalFailure, unit, Unit } from './models';
import * as t from 'io-ts';
import { PathReporter } from 'io-ts/lib/PathReporter';
import * as either from 'fp-ts/lib/Either';
import { Either } from 'fp-ts/lib/Either';
import * as taskEither from 'fp-ts/lib/TaskEither';
import { TaskEither } from 'fp-ts/lib/TaskEither';

export interface ResponseType<T, L, R> {
    match: (response: Response) => boolean;
    validator: t.Type<T>;
    chain: (response: T) => TaskEither<L, R>;
}

export function fetchJson<T1, L1, R1, T2, L2, R2, T3, L3, R3>(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: [ResponseType<T1, L1, R1>, ResponseType<T2, L2, R2>, ResponseType<T3, L3, R3>]): TaskEither<Unit | GeneralFailure | L1 | L2 | L3, R1 | R2 | R3>;
export function fetchJson<T1, L1, R1, T2, L2, R2>(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: [ResponseType<T1, L1, R1>, ResponseType<T2, L2, R2>]): TaskEither<Unit | GeneralFailure | L1 | L2, R1 | R2>;
export function fetchJson<T1, L1, R1>(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: [ResponseType<T1, L1, R1>]): TaskEither<Unit | GeneralFailure | L1, R1>;
export function fetchJson(input: Request | string, init: RequestInit, actionDescription: string, responseTypes: ResponseType<any, any, any>[]): TaskEither<any, any> {
    return taskEither.tryCatch(
        () => fetch(input, init),
        (error: any) => mapFetchError(error, actionDescription))
        .chain(response => {
            const contentType = response.headers.get("content-type");
            if (contentType === "application/json") {

                const matchingResponseType = responseTypes.find(responseType => responseType.match(response));
                if (matchingResponseType !== undefined) {
                    return parseJsonResponse(response, matchingResponseType.validator, actionDescription)
                        .chain(matchingResponseType.chain);
                }

                return parseTextResponse(response, actionDescription)
                    .chain(body => taskEither.fromLeft(Errors.Internet.unexpectedResponse(actionDescription, {
                        message: `Response with unexpected status code '${response.status}'`,
                        body: body
                    })));
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
        () => response.text(),
        (error: any) =>
            Errors.Internet.unexpectedResponse(actionDescription, {
                message: `Calling response.text returned an error on response with status code '${response.status}'`,
                error
            }));
}


function parseJsonResponse<T>(response: Response, validator: t.Type<T>, actionDescription: string): TaskEither<GeneralFailure, T> {
    return taskEither.tryCatch(
        () => response.json(),
        (error: any) =>
            Errors.Internet.unexpectedResponse(actionDescription, {
                message: "Calling response.json returned an error on response with status code '${response.status}'",
                error
            }))
        .chain(data =>
            taskEither.fromEither(validateObject(data, validator, actionDescription)));
}

function validateObject<T>(data: any, validator: t.Type<T>, actionDescription: string): Either<GeneralFailure, T> {
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
        return unit;
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
                    reason: "Our server returned an unexpected response or someone else's server responded (this would be weird)"
                },
                possibleSolutions: ["This is probably our fault, so we'll need to fix it"],
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