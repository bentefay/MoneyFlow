import { Observable, from, of, empty } from 'rxjs';
import { mergeMap, filter, mergeAll, withLatestFrom } from 'rxjs/operators';
import { authActions, AuthAction, Email, Password, EncryptedVault, LoginError, AuthStateValue } from '.';
import { isActionOf } from 'typesafe-actions';
import { RootState } from '../store';
import bcrypt from "bcryptjs";
import * as t from 'io-ts';
import * as taskEither from 'fp-ts/lib/TaskEither';
import { TaskEither } from 'fp-ts/lib/TaskEither';
import { Unit, Errors, GeneralFailure, validationFailure, unit } from '../shared/models';
import { fetchJson } from '../shared/http';
import { loginErrored, userAccountNotFound, loginSucceeded } from './actions';
import { identity } from 'fp-ts/lib/function';
import { isEmpty } from 'lodash';
import { apiBaseUrl } from '../../config';

const { hash } = bcrypt;

(window as any).bcrypt = bcrypt;

export function login(action: Observable<AuthAction>, state: Observable<RootState>): Observable<AuthAction> {
    return action.pipe(
        filter(isActionOf(authActions.loginInitiated)),
        withLatestFrom(state),
        mergeMap(([_, state]) =>
            state.auth.value.email != null && state.auth.value.password != null && isEmpty(state.auth.errors.email) && isEmpty(state.auth.errors.password) ?
                of({ email: state.auth.value.email, password: state.auth.value.password }) :
                empty()),
        mergeMap(({ email, password }) => {
            return from(
                getSalt(email)
                    .mapLeft<Unit | AuthAction>(mapLoginErrorToAction)
                    .chain(salt =>
                        salt != null ?
                            toHash(password, salt).mapLeft(mapLoginErrorToAction) :
                            taskEither.asLeft(userAccountNotFound({ email, password })))
                    .chain(hashedPassword =>
                        getVault(email, hashedPassword)
                            .bimap(
                                mapLoginErrorToAction,
                                vault => loginSucceeded(vault)
                            ))
                    .fold(identity, identity)
                    .map(action => action === unit ? empty() : of(action))
                    .run())
                .pipe(mergeAll());
        }));
}

function mapLoginErrorToAction(error: LoginError) {
    return error == unit ? unit : loginErrored(error);
}

const GetVaultSuccessResponse = t.type({
    data: t.string
});

const GetVaultValidationErrorResponse = t.type({
    validationErrors: t.type({
        email: t.array(t.string),
        password: t.array(t.string)
    })
});

function getVault(email: Email, hashedPassword: HashedPassword): TaskEither<LoginError, EncryptedVault> {
    return fetchJson(
        `${apiBaseUrl}/api/vault`,
        { headers: authHeader({ email: email.value, password: hashedPassword.value }) },
        "Retrieving your data",
        [{
            match: ({ status }) => status == 200,
            validator: GetVaultSuccessResponse,
            chain: ({ data }) => taskEither.asRight(new EncryptedVault(data))
        },
        {
            match: ({ status }) => status == 400,
            validator: GetVaultValidationErrorResponse,
            chain: ({ validationErrors }) => taskEither.asLeft(validationFailure<AuthStateValue>({ errors: validationErrors }))
        }]
    )
}

const GetSaltSuccessResponse = t.type({
    salt: t.union([t.null, t.string])
});

const GetSaltValidationErrorResponse = t.type({
    validationErrors: t.type({
        email: t.array(t.string)
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

function getSalt(email: Email): TaskEither<LoginError, PasswordSalt | null> {
    return fetchJson(
        `${apiBaseUrl}/api/salt`,
        { headers: authHeader({ email: email.value }) },
        "Checking for your account",
        [{
            match: ({ status }) => status == 200,
            validator: GetSaltSuccessResponse,
            chain: ({ salt }) => taskEither.asRight(salt != null ? new PasswordSalt(salt) : null)
        },
        {
            match: ({ status }) => status == 400,
            validator: GetSaltValidationErrorResponse,
            chain: ({ validationErrors }) => taskEither.asLeft(validationFailure<AuthStateValue>({ errors: validationErrors }))
        }]);
}


function toHash(password: Password, salt: PasswordSalt): TaskEither<GeneralFailure, HashedPassword> {
    return taskEither.tryCatch(
        () => hash(password.value, salt.value),
        (error: any) => Errors.unexpected("Encrypting your password to send to our server", error))
        .map(hashedPassword => new HashedPassword(hashedPassword));
}

function authHeader(obj: any) {
    return { "Authorization": `Basic ${base64(obj)}` };
}

function base64(obj: any) {
    return btoa(JSON.stringify(obj))
}