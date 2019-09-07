import { GeneralFailure, Unit, ValidationErrors, Invalid } from '../shared/models';

export type LoginError = GeneralFailure | Invalid<AuthStateValue> | Unit;

export enum AuthView {
    logIn,
    createAccount
}

export interface AuthState extends Readonly<{
    view: AuthView,
    value: AuthStateValue,
    errors: ValidationErrors<AuthStateValue>,
    generalFailure?: GeneralFailure,
    isLoading: boolean
}> { }

export interface AuthStateValue extends Readonly<{
    email: Email | null,
    password: Password | null
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
