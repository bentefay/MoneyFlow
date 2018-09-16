export type Omit<T, K> = Pick<T, Exclude<keyof T, K>>

export type ValidationErrors<T> =
    T extends any[] ? ArrayValidationErrors<T[number]> :
    T extends object ? ObjectValidationErrors<T> :
    string[]

export type NonFunctionKeys<T> = {
    [K in keyof T]: T[K] extends Function ? never : K
}[keyof T];

export type ObjectValidationErrors<T> = {
    readonly [P in NonFunctionKeys<T>]?: ValidationErrors<T[P]>
}

export interface ArrayValidationErrors<T> extends ReadonlyArray<ValidationErrors<T>> { }

export type Untagged<T> = Omit<T, "type">;

export function tagged<T extends object & { type: string }>(type: T["type"]) {
    return (value: Untagged<T>): T => Object.assign({}, value, { type: type }) as any;
}

export type Unit = [];

export const unit: Unit = [];

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

export module Errors {
    export function unexpected(actionDescription: string, error: unknown) {
        return generalFailure({
            friendly: {
                actionDescription: actionDescription,
                reason: "An unexpected error occurred"
            },
            error: error
        });
    }
}