import { ValidationErrors, tagged, Untagged } from '../shared/model';

export interface Invalid<T> {
    type: "Invalid";
    errors: ValidationErrors<T>;
}

export function validationFailure<T>(value: Untagged<Invalid<T>>): Invalid<T> {
    return { ...value, type: "Invalid" }
}

export interface GeneralFailure {
    type: "GeneralFailure",
    friendly: {
        /** A present tense description of the action that failed (e.g. "Logging in", "Retrieving vault")  */
        actionDescription: string,
        reason: string
    };
    possibleSolutions?: string[];
    error: any;
}

export const generalFailure = tagged<GeneralFailure>("GeneralFailure");

export interface AuthState extends Readonly<{
    username: string | null,
    password: string | null
}> { }
