
export type Omit<T, K> = Pick<T, Exclude<keyof T, K>>

export type ValidationErrors<T> = {
    readonly [P in keyof T]?:
    T[P] extends (infer U)[] ? (U extends string | number ? string[] : ValidationErrors<U>[]) :
    T[P] extends object ? ValidationErrors<T[P]> :
    string[];
}

export interface ValidationFailure<T> {
    type: "ValidationFailure";
    errors: ValidationErrors<T>;
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

function taggedType<T extends object & { type: string }>(type: T["type"]) {
    return (value: Omit<T, "type">): T => Object.assign({}, value, { type: type }) as any;
}

export const generalFailure = taggedType<GeneralFailure>("GeneralFailure");

export function validationFailure<T>(value: Omit<ValidationFailure<T>, "type">): ValidationFailure<T> {
    return { ...value, type: "ValidationFailure" }
}

export type AuthState = Readonly<{
    username: string | null,
    password: string | null
}>;
