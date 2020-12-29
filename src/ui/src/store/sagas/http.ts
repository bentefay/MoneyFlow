import { Email } from "../auth";
import * as t from "io-ts";
import * as taskEither from "fp-ts/lib/TaskEither";
import "../shared/taskEither";
import { TaskEither } from "fp-ts/lib/TaskEither";
import { GeneralError, Unit, newGeneralError, unit, EncryptedVault, NewVaultPlaceholder, UserId, ETag } from "../shared/models";
import { fetchJson, Aborted } from "../shared/http";
import { apiBaseUrl } from "../../config";
import { HashedPassword, createAuthHeader } from "../auth/crypto";
import _ from "lodash";
import { Errors, CredentialsIncorrectError, AccountAlreadyExistsError } from "../auth/errors";

const CreateUserResponse = t.type({
    userId: t.string,
});

export type CreateUserError = AccountAlreadyExistsError | GeneralError | Aborted;

export function createUser(email: Email, hashedPassword: HashedPassword): TaskEither<CreateUserError, NewVaultPlaceholder> {
    const actionDescription = "Creating your account";
    return fetchJson(
        `${apiBaseUrl}/api/users`,
        {
            headers: createAuthHeader({
                email: email.value,
                hashedPassword: hashedPassword.value,
            }),
            method: "put",
        },
        actionDescription,
        [
            {
                match: ({ status }) => status == 201,
                validator: CreateUserResponse,
                chain: ({ userId }) => taskEither.right(new NewVaultPlaceholder(new UserId(userId))),
            },
            {
                match: ({ status }) => status == 409,
                validator: t.string,
                chain: (description) => taskEither.left(Errors.Auth.accountAlreadyExists(description)),
            },
            {
                match: ({ status }) => _.includes([400, 500], status),
                validator: t.string,
                chain: (reason) =>
                    taskEither.left(
                        newGeneralError({
                            friendly: {
                                actionDescription: actionDescription,
                                reason: reason,
                            },
                            error: reason,
                        })
                    ),
            },
        ]
    );
}

const GetVaultResponse = t.type({
    userId: t.string,
    content: t.string,
    eTag: t.string,
});

export type GetVaultError = CredentialsIncorrectError | GeneralError | Aborted;

export function getVault(email: Email, hashedPassword: HashedPassword): TaskEither<GetVaultError, EncryptedVault | NewVaultPlaceholder> {
    const actionDescription = "Retrieving your account data";
    return fetchJson(
        `${apiBaseUrl}/api/vaults`,
        {
            headers: createAuthHeader({
                email: email.value,
                hashedPassword: hashedPassword.value,
            }),
        },
        actionDescription,
        [
            {
                match: ({ status }) => status == 200,
                validator: GetVaultResponse,
                chain: ({ userId, content, eTag }) => taskEither.right(new EncryptedVault(new UserId(userId), new ETag(eTag), content)),
            },
            {
                match: ({ status }) => status == 404,
                validator: CreateUserResponse,
                chain: ({ userId }) => taskEither.right(new NewVaultPlaceholder(new UserId(userId))),
            },
            {
                match: ({ status }) => status == 401,
                validator: t.string,
                chain: (description) => taskEither.left(Errors.Auth.credentialsIncorrect(description)),
            },
            {
                match: ({ status }) => _.includes([400, 500], status),
                validator: t.string,
                chain: (reason) =>
                    taskEither.left(
                        newGeneralError({
                            friendly: {
                                actionDescription: actionDescription,
                                reason: reason,
                            },
                            error: reason,
                        })
                    ),
            },
        ]
    );
}

export function updateVault(email: Email, hashedPassword: HashedPassword): TaskEither<GeneralError | Aborted, Unit> {
    const actionDescription = "Updating your account data";
    return fetchJson(
        `${apiBaseUrl}/api/vaults`,
        {
            headers: createAuthHeader({
                email: email.value,
                hashedPassword: hashedPassword.value,
            }),
            method: "put",
            body: JSON.stringify({}),
        },
        actionDescription,
        [
            {
                match: ({ status }) => status == 200,
                validator: t.unknown,
                chain: () => taskEither.right(unit),
            },
            {
                match: ({ status }) => _.includes([400, 401, 409, 500], status),
                validator: t.string,
                chain: (reason) =>
                    taskEither.left(
                        newGeneralError({
                            friendly: {
                                actionDescription: actionDescription,
                                reason: reason,
                            },
                            error: reason,
                        })
                    ),
            },
        ]
    );
}
