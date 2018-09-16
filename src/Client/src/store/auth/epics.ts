import { Observable, from, of } from 'rxjs';
import { mergeMap, map, filter, withLatestFrom, catchError } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import { authActions, AuthAction, Failure, GeneralFailure, generalFailure, AuthState, validationFailure, Invalid } from '.';
import { isActionOf, action } from 'typesafe-actions';
import { RootState } from '../store';
import { loginOrCreateCompleted, loginOrCreateErrored } from './actions';
import { hash } from "bcryptjs";
import * as t from 'io-ts';
import either, { Either } from 'fp-ts/lib/Either';
import taskEither, { TaskEither } from 'fp-ts/lib/TaskEither';
import option, { Option } from 'fp-ts/lib/Option';
import { Unit, Errors, GeneralFailure } from '../shared/model';
import { fetchJson } from '../shared/http';

export function getVault2(action: Observable<AuthAction>, state: Observable<RootState>) {
    return action.pipe(
        filter(isActionOf(authActions.loginOrCreateInitiated)),
        withLatestFrom(state),
        map(([_, { auth }]) => auth),
        mergeMap(auth => {
            getSalt(username).
                .chain(salt => {
                    toHash(password, salt)
                });
        });
}

const GetVaultSuccessResponse = t.type({
    data: t.string
});

const GetVaultValidationErrorResponse = t.type({
    validationErrors: t.type({
        username: t.array(t.string),
        password: t.array(t.string)
    })
});

function getVaultFromHashedPassword(username: string, hashedPassword: string) {
    return fetchJson(
        `api/vault`,
        { headers: authHeader({ username, password: hashedPassword }) },
        "Retrieving your data",
        [{
            match: ({ status }) => status == 200,
            validator: GetVaultSuccessResponse,
            chain: ({ data }) => taskEither.asRight(data)
        },
        {
            match: ({ status }) => status == 400,
            validator: GetVaultValidationErrorResponse,
            chain: ({ validationErrors }) => taskEither.asLeft(validationFailure<AuthState>({ errors: validationErrors }))
        }]
    )
}

const GetSaltSuccessResponse = t.type({
    salt: t.union([t.null, t.string])
});

const GetSaltValidationErrorResponse = t.type({
    validationErrors: t.type({
        username: t.array(t.string)
    })
});

export class Salt {
    constructor(public readonly value: string) { }
}

export type SaltError = GeneralFailure | Invalid<AuthState> | Unit;

function getSalt(username: string): TaskEither<SaltError, Option<Salt>> {
    return fetchJson(
        `/api/salt`,
        { headers: authHeader({ username }) },
        "Checking for your account",
        [{
            match: ({ status }) => status == 200,
            validator: GetSaltSuccessResponse,
            chain: ({ salt }) => taskEither.asRight(salt != null ? option.some(new Salt(salt)) : option.none)
        },
        {
            match: ({ status }) => status == 400,
            validator: GetSaltValidationErrorResponse,
            chain: ({ validationErrors }) => taskEither.asLeft(validationFailure<AuthState>({ errors: validationErrors }))
        }]);
}


function toHash(password: string, salt: string): TaskEither<GeneralFailure, string> {
    return taskEither.tryCatch(
        () => hash(password, salt),
        (error: any) => Errors.unexpected("Encrypting your password", error));
}

function authHeader(obj: any) {
    return { "Authorization": `Basic ${base64(obj)}` };
}

function base64(obj: any) {
    return btoa(JSON.stringify(obj))
}