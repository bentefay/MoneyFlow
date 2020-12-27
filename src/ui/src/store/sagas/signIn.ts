import { authActions, Email, AuthAction } from "../auth";
import { isActionOf } from "typesafe-actions";
import { takeLatest, put } from "redux-saga/effects";
import { pipe } from "fp-ts/lib/pipeable";
import * as t from "io-ts";
import * as taskEither from "fp-ts/lib/TaskEither";
import "../shared/taskEither";
import { TaskEither } from "fp-ts/lib/TaskEither";
import { GeneralError, Unit, newGeneralError, unit, EncryptedVault, NewVaultPlaceholder, UserId, ETag } from "../shared/models";
import { fetchJson, Aborted, aborted } from "../shared/http";
import { apiBaseUrl } from "../../config";
import { taskAsGenerator } from "../shared/sagas";
import { HashedPassword, hashPassword, authHeader } from "../auth/crypto";
import _, { identity } from "lodash";
import { Errors, CredentialsIncorrectError, AccountAlreadyExistsError } from "../auth/errors";

export function* signIn() {
    yield takeLatest(isActionOf(authActions.signInInitiated), function*({ payload }) {
        const {
            credentials: { email, password },
            create
        } = payload;

        const task = pipe(
            hashPassword(password),
            taskEither.chain(hashedPassword => getOrCreateVault(create, email, hashedPassword)),
            taskEither.match(
                error => {
                    return error == aborted ? authActions.requestAborted() : authActions.signInErrored(error);
                },
                vault => {
                    return identity<AuthAction>(authActions.signInSucceeded(vault));
                }
            )
        );

        const action = yield* taskAsGenerator(task);

        yield put(action);
    });
}

export function getOrCreateVault(
    create: boolean,
    email: Email,
    hashedPassword: HashedPassword
): TaskEither<CreateUserError | GetVaultError, EncryptedVault | NewVaultPlaceholder> {
    return create ? createUser(email, hashedPassword) : getVault(email, hashedPassword);
}

const CreateUserResponse = t.type({
    userId: t.string
});

export type CreateUserError = AccountAlreadyExistsError | GeneralError | Aborted;

function createUser(email: Email, hashedPassword: HashedPassword): TaskEither<CreateUserError, NewVaultPlaceholder> {
    const actionDescription = "Creating your account";
    return fetchJson(
        `${apiBaseUrl}/api/users`,
        {
            headers: authHeader({
                email: email.value,
                hashedPassword: hashedPassword.value
            }),
            method: "put"
        },
        actionDescription,
        [
            {
                match: ({ status }) => status == 201,
                validator: CreateUserResponse,
                chain: ({ userId }) => taskEither.right(new NewVaultPlaceholder(new UserId(userId)))
            },
            {
                match: ({ status }) => status == 409,
                validator: t.string,
                chain: description => taskEither.left(Errors.Auth.accountAlreadyExists(description))
            },
            {
                match: ({ status }) => _.includes([400, 500], status),
                validator: t.string,
                chain: reason =>
                    taskEither.left(
                        newGeneralError({
                            friendly: {
                                actionDescription: actionDescription,
                                reason: reason
                            },
                            error: reason
                        })
                    )
            }
        ]
    );
}

const GetVaultResponse = t.type({
    userId: t.string,
    content: t.string,
    eTag: t.string
});

export type GetVaultError = CredentialsIncorrectError | GeneralError | Aborted;

function getVault(email: Email, hashedPassword: HashedPassword): TaskEither<GetVaultError, EncryptedVault | NewVaultPlaceholder> {
    const actionDescription = "Retrieving your account data";
    return fetchJson(
        `${apiBaseUrl}/api/vaults`,
        {
            headers: authHeader({
                email: email.value,
                hashedPassword: hashedPassword.value
            })
        },
        actionDescription,
        [
            {
                match: ({ status }) => status == 200,
                validator: GetVaultResponse,
                chain: ({ userId, content, eTag }) => taskEither.right(new EncryptedVault(new UserId(userId), new ETag(eTag), content))
            },
            {
                match: ({ status }) => status == 404,
                validator: CreateUserResponse,
                chain: ({ userId }) => taskEither.right(new NewVaultPlaceholder(new UserId(userId)))
            },
            {
                match: ({ status }) => status == 401,
                validator: t.string,
                chain: description => taskEither.left(Errors.Auth.credentialsIncorrect(description))
            },
            {
                match: ({ status }) => _.includes([400, 500], status),
                validator: t.string,
                chain: reason =>
                    taskEither.left(
                        newGeneralError({
                            friendly: {
                                actionDescription: actionDescription,
                                reason: reason
                            },
                            error: reason
                        })
                    )
            }
        ]
    );
}

function updateVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralError | Aborted, Unit> {
    const actionDescription = "Updating your account data";
    return fetchJson(
        `${apiBaseUrl}/api/vaults`,
        {
            headers: authHeader({
                email: email.value,
                hashedPassword: hashedPassword.value
            }),
            method: "put",
            body: JSON.stringify({})
        },
        actionDescription,
        [
            {
                match: ({ status }) => status == 200,
                validator: t.unknown,
                chain: () => taskEither.right(unit)
            },
            {
                match: ({ status }) => _.includes([400, 401, 409, 500], status),
                validator: t.string,
                chain: reason =>
                    taskEither.left(
                        newGeneralError({
                            friendly: {
                                actionDescription: actionDescription,
                                reason: reason
                            },
                            error: reason
                        })
                    )
            }
        ]
    );
}