import { Untagged, NonFunctionKeys } from '.';
import { ReactElement, Key } from 'react';

export type ValidationError = string | (ReactElement<any> & { key: Key });

export type ValidationErrors<T> =
    T extends any[] ? ArrayValidationErrors<T[number]> :
    T extends { type: any, value: string | number } ? ReadonlyArray<ValidationError> :
    T extends object ? ObjectValidationErrors<T> :
    ReadonlyArray<ValidationError>

export type ObjectValidationErrors<T> = {
    readonly [P in NonFunctionKeys<T>]?: ValidationErrors<T[P]>
}

export interface ArrayValidationErrors<T> extends ReadonlyArray<ValidationErrors<T>> { }

export interface Invalid<T> extends Readonly<{
    type: "invalid";
    errors: ValidationErrors<T>;
}> { }

export function validationFailure<T>(value: Untagged<Invalid<T>>): Invalid<T> {
    return { ...value, type: "invalid" }
}