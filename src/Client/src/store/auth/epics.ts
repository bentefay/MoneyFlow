import { Observable, from, of, empty } from 'rxjs';
import { mergeMap, filter, mergeAll } from 'rxjs/operators';
import { authActions, AuthAction, AuthState, Username, Password, EncryptedVault } from '.';
import { isActionOf } from 'typesafe-actions';
import { RootState } from '../store';
import { hash } from "bcryptjs";
import * as t from 'io-ts';
import taskEither, { TaskEither } from 'fp-ts/lib/TaskEither';
import { Unit, Errors, GeneralFailure, Invalid, validationFailure, unit } from '../shared/model';
import { fetchJson } from '../shared/http';
import { loginErrored, userAccountNotFound, loginSucceeded } from './actions';
import { identity } from 'fp-ts/lib/function';

export function login(action: Observable<AuthAction>, state: Observable<RootState>): Observable<AuthAction> {
    return action.pipe(
        filter(isActionOf(authActions.loginInitiated)),
        mergeMap(({ payload: { username, password } }) => {
            return from(
                getSalt(username)
                    .mapLeft<Unit | AuthAction>(mapSaltErrorToAction)
                    .chain(salt =>
                        salt != null ?
                            toHash(password, salt).mapLeft(mapSaltErrorToAction) :
                            taskEither.asLeft(userAccountNotFound({ username, password })))
                    .chain(hashedPassword =>
                        getVault(username, hashedPassword)
                            .bimap(
                                mapSaltErrorToAction,
                                vault => loginSucceeded(vault)
                            ))
                    .fold(identity, identity)
                    .map(action => action == unit ? empty() : of(action))
                    .run())
                .pipe(mergeAll());
        }));
}

function mapSaltErrorToAction(error: SaltError) {
    return error == unit ? unit : loginErrored(error);
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

function getVault(username: Username, hashedPassword: HashedPassword) {
    return fetchJson(
        `api/vault`,
        { headers: authHeader({ username: username.value, password: hashedPassword.value }) },
        "Retrieving your data",
        [{
            match: ({ status }) => status == 200,
            validator: GetVaultSuccessResponse,
            chain: ({ data }) => taskEither.asRight(new EncryptedVault(data))
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

export class PasswordSalt {
    public type = PasswordSalt;
    constructor(public readonly value: string) { }
}

export class HashedPassword {
    public type = HashedPassword;
    constructor(public readonly value: string) { }
}

export type SaltError = GeneralFailure | Invalid<AuthState> | Unit;

function getSalt(username: Username): TaskEither<SaltError, PasswordSalt | null> {
    return fetchJson(
        `/api/salt`,
        { headers: authHeader({ username: username.value }) },
        "Checking for your account",
        [{
            match: ({ status }) => status == 200,
            validator: GetSaltSuccessResponse,
            chain: ({ salt }) => taskEither.asRight(salt != null ? new PasswordSalt(salt) : null)
        },
        {
            match: ({ status }) => status == 400,
            validator: GetSaltValidationErrorResponse,
            chain: ({ validationErrors }) => taskEither.asLeft(validationFailure<AuthState>({ errors: validationErrors }))
        }]);
}


function toHash(password: Password, salt: PasswordSalt): TaskEither<GeneralFailure, HashedPassword> {
    return taskEither.tryCatch(
        () => hash(password.value, salt.value),
        (error: any) => Errors.unexpected("Encrypting your password", error))
        .map(hashedPassword => new HashedPassword(hashedPassword));
}

function authHeader(obj: any) {
    return { "Authorization": `Basic ${base64(obj)}` };
}

function base64(obj: any) {
    return btoa(JSON.stringify(obj))
}