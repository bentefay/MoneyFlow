import { authActions, AuthAction, Email, EncryptedVault } from '.';
import { isActionOf } from 'typesafe-actions';
import { takeLatest, put } from 'redux-saga/effects'
import { pipe } from 'fp-ts/lib/pipeable'
import * as t from 'io-ts';
import * as taskEither from 'fp-ts/lib/TaskEither';
import * as either from 'fp-ts/lib/Either';
import { TaskEither } from 'fp-ts/lib/TaskEither';
import { GeneralFailure } from '../shared/models';
import { fetchJson, Aborted, aborted } from '../shared/http';
import { loginErrored, loginSucceeded } from './actions';
import { apiBaseUrl } from '../../config';
import { promiseAsGenerator } from '../shared/sagas';
import { HashedPassword, hashPassword, authHeader } from './crypto';

export function* login() {
    yield takeLatest(isActionOf(authActions.loginInitiated), function* ({ payload }) {
        const { credentials: { email, password }, create } = payload;

        const vaultOrError = yield* promiseAsGenerator(
            pipe(
                hashPassword(password),
                taskEither.chain(hashedPassword =>
                    create ?
                        createVault(email, hashedPassword) :
                        getVault(email, hashedPassword)
                )
            )()
        );

        if (either.isLeft(vaultOrError)) {
            if (vaultOrError.left !== aborted)
                yield put(<AuthAction>loginErrored(vaultOrError.left));
        } else {
            yield put(loginSucceeded(vaultOrError.right));
        }
    });
}

const GetVaultSuccessResponse = t.type({
    data: t.string
});

function createVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralFailure | Aborted, EncryptedVault> {
    return fetchJson(
        `${apiBaseUrl}/api/vault/new`,
        { headers: authHeader({ email: email.value, password: hashedPassword.value }), method: "put" },
        "Creating your account",
        [{
            match: ({ status }) => status == 200,
            validator: GetVaultSuccessResponse,
            chain: ({ data }) => taskEither.right(new EncryptedVault(data))
        }]
    )
}

function getVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralFailure | Aborted, EncryptedVault> {
    return fetchJson(
        `${apiBaseUrl}/api/vault`,
        { headers: authHeader({ email: email.value, password: hashedPassword.value }) },
        "Retrieving your account data",
        [{
            match: ({ status }) => status == 200,
            validator: GetVaultSuccessResponse,
            chain: ({ data }) => taskEither.right(new EncryptedVault(data))
        }]
    )
}
