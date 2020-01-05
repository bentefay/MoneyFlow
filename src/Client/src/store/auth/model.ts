import { GeneralError } from "../shared/models";
import { CredentialsIncorrectError, AccountAlreadyExistsError } from "./errors";

export type AuthStateError = AccountAlreadyExistsError | CredentialsIncorrectError | GeneralError;

export interface AuthState
    extends Readonly<{
        createAccount: boolean;
        credentials?: UserCredentials;
        error?: AuthStateError;
        isLoading: boolean;
    }> {}

export interface UserCredentials
    extends Readonly<{
        email: Email;
        password: Password;
    }> {}

export class Email {
    public type = Email;
    constructor(public readonly value: string) {}
}

export class Password {
    public type = Password;
    constructor(public readonly value: string) {}
}

export class EncryptedVault {
    public type = EncryptedVault;
    constructor(public readonly userId: UserId, public readonly eTag: ETag, public readonly content: string) {}
}

export class NewVaultPlaceholder {
    public type = NewVaultPlaceholder;
    constructor(public readonly userId: UserId) {}
}

export class UserId {
    public type = UserId;
    constructor(public readonly value: string) {}
}

export class ETag {
    public type = ETag;
    constructor(public readonly value: string) {}
}
