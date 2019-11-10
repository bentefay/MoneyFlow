import { authActions, Email, AuthAction } from '.';
import { isActionOf } from 'typesafe-actions';
import { takeLatest, put } from 'redux-saga/effects'
import { pipe } from 'fp-ts/lib/pipeable'
import * as t from 'io-ts';
import * as taskEither from 'fp-ts/lib/TaskEither';
import { TaskEither } from 'fp-ts/lib/TaskEither';
import { GeneralFailure, Unit, generalFailure, unit } from '../shared/models';
import { fetchJson, Aborted, aborted } from '../shared/http';
import { loginErrored, loginSucceeded, requestAborted } from './actions';
import { apiBaseUrl } from '../../config';
import { taskAsGenerator } from '../shared/sagas';
import { HashedPassword, hashPassword, authHeader } from './crypto';
import _, { identity } from 'lodash';
import { EncryptedVault, ETag, UserId, NewVaultPlaceholder } from './model';

export function* login() {
    yield takeLatest(isActionOf(authActions.loginInitiated), function* ({ payload }) {
        const { credentials: { email, password }, create } = payload;

        const action = yield* taskAsGenerator(
            pipe(
                hashPassword(password),
                taskEither.chain(hashedPassword =>
                    create ?
                        pipe(
                            createVault(email, hashedPassword),
                            taskEither.map(x => identity<EncryptedVault | NewVaultPlaceholder>(x))) :
                        pipe(
                            getVault(email, hashedPassword),
                            taskEither.map(x => identity<EncryptedVault | NewVaultPlaceholder>(x)))
                ),
                taskEither.fold(
                    error => {
                        return error == aborted ?
                            identity<AuthAction>(requestAborted()) :
                            identity<AuthAction>(loginErrored(error));
                    },
                    vault => {
                        return identity<AuthAction>(loginSucceeded(vault));
                    }
                )
            )
        );

        yield put(action);
    });
}

const CreateVaultResponse = t.type({
    userId: t.string
});

function createVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralFailure | Aborted, NewVaultPlaceholder> {
    const actionDescription = "Creating your account";
    return fetchJson(
        `${apiBaseUrl}/api/vault/new`,
        {
            headers: authHeader({ email: email.value, password: hashedPassword.value }),
            method: "put"
        },
        actionDescription,
        [{
            match: ({ status }) => status == 201,
            validator: CreateVaultResponse,
            chain: ({ userId }) => taskEither.right(new NewVaultPlaceholder(new UserId(userId))),

        },
        {
            match: ({ status }) => _.includes([400, 401, 409, 500], status),
            validator: t.string,
            chain: reason => taskEither.left(
                generalFailure({
                    friendly: { actionDescription: actionDescription, reason: reason },
                    error: reason
                }))
        }]
    )
}

const GetVaultResponse = t.type({
    userId: t.string,
    content: t.string,
    eTag: t.string
});

function getVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralFailure | Aborted, EncryptedVault> {
    const actionDescription = "Retrieving your account data";
    return fetchJson(
        `${apiBaseUrl}/api/vault`,
        {
            headers: authHeader({ email: email.value, password: hashedPassword.value })
        },
        actionDescription,
        [{
            match: ({ status }) => status == 200,
            validator: GetVaultResponse,
            chain: ({ userId, content, eTag }) => taskEither.right(new EncryptedVault(new UserId(userId), new ETag(eTag), content))
        }, {
            match: ({ status }) => _.includes([400, 401, 409, 500], status),
            validator: t.string,
            chain: reason => taskEither.left(
                generalFailure({
                    friendly: { actionDescription: actionDescription, reason: reason },
                    error: reason
                }))
        }
        ]
    )
}

function updateVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralFailure | Aborted, Unit> {
    const actionDescription = "Updating your account data";
    return fetchJson(
        `${apiBaseUrl}/api/vault`,
        {
            headers: authHeader({ email: email.value, password: hashedPassword.value }),
            method: "put",
            body: JSON.stringify({})
        },
        actionDescription,
        [{
            match: ({ status }) => status == 200,
            validator: t.unknown,
            chain: () => taskEither.right(unit)
        }, {
            match: ({ status }) => _.includes([400, 401, 409, 500], status),
            validator: t.string,
            chain: reason => taskEither.left(
                generalFailure({
                    friendly: { actionDescription: actionDescription, reason: reason },
                    error: reason
                }))
        }]
    )
}