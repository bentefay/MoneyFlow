import { GeneralFailure, Unit, Invalid } from '../shared/models';

export type LoginError = GeneralFailure | Invalid<UserCredentials> | Unit;

export interface AuthState extends Readonly<{
    createAccount: boolean,
    credentials: UserCredentials | null,
    generalFailure?: GeneralFailure,
    isLoading: boolean
}> { }

export interface UserCredentials extends Readonly<{
    email: Email,
    password: Password
}> { }

export class Email {
    public type = Email;
    constructor(public readonly value: string) { }
}

export class Password {
    public type = Password;
    constructor(public readonly value: string) { }
}

export class EncryptedVault {
    public type = EncryptedVault;
    constructor(public readonly value: string) { }
}
