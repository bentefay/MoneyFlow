import { authActions, Email, AuthAction } from "../auth";
import { isActionOf } from "typesafe-actions";
import { takeLatest, put } from "redux-saga/effects";
import { pipe } from "fp-ts/lib/pipeable";
import * as taskEither from "fp-ts/lib/TaskEither";
import "../shared/taskEither";
import { TaskEither } from "fp-ts/lib/TaskEither";
import { EncryptedVault, NewVaultPlaceholder } from "../shared/models";
import { aborted } from "../shared/http";
import { taskAsGenerator } from "../shared/sagas";
import { HashedPassword, hashPassword } from "../auth/crypto";
import _, { identity } from "lodash";
import { createUser, CreateUserError, getVault, GetVaultError } from "./http";

export function* signIn() {
    yield takeLatest(isActionOf(authActions.signInInitiated), function* ({ payload }) {
        const {
            credentials: { email, password },
            create,
        } = payload;

        const task = pipe(
            hashPassword(password),
            taskEither.chain((hashedPassword) => getOrCreateVault(create, email, hashedPassword)),
            taskEither.match(
                (error) => {
                    return error == aborted ? authActions.requestAborted() : authActions.signInErrored(error);
                },
                (vault) => {
                    return identity<AuthAction>(authActions.signInSucceeded(vault));
                }
            )
        );

        const action = yield* taskAsGenerator(task);

        yield put(action);
    });
}

function getOrCreateVault(
    create: boolean,
    email: Email,
    hashedPassword: HashedPassword
): TaskEither<CreateUserError | GetVaultError, EncryptedVault | NewVaultPlaceholder> {
    return create ? createUser(email, hashedPassword) : getVault(email, hashedPassword);
}
