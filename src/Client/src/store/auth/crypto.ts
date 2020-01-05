import bcrypt from "bcryptjs";
import { Password } from ".";
import { pipe } from "fp-ts/lib/pipeable";
import * as taskEither from "fp-ts/lib/TaskEither";
import { TaskEither } from "fp-ts/lib/TaskEither";
import { Errors, GeneralError } from "../shared/models";

(window as any).bcrypt = bcrypt;

export class HashedPassword {
    public type = HashedPassword;
    constructor(public readonly value: string) {}
}

const salt = "$2a$10$SweJ37PLcqGyrhjb24gwPu";

export function hashPassword(password: Password): TaskEither<GeneralError, HashedPassword> {
    return pipe(
        hash(password.value, salt, "Encrypting your password to send to our server"),
        taskEither.map(hashedPassword => new HashedPassword(hashedPassword))
    );
}

function hash(value: string, salt: string, description: string): TaskEither<GeneralError, string> {
    return taskEither.tryCatch(
        () => bcrypt.hash(value, salt),
        (error: any) => Errors.unexpected(description, error)
    );
}

export function authHeader(obj: { email: string; hashedPassword: string }) {
    return { Authorization: `Bearer ${base64(obj)}` };
}

function base64(obj: any) {
    return btoa(JSON.stringify(obj));
}
