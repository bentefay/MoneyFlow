import * as React from "react";
import { Email, Password } from '.';
import { includes, flatten, some } from 'lodash';
import { ValidationError } from '../shared/models';

export function validateEmail(email: Email): ValidationError[] {
    if (email.value.length == 0)
        return ["We need an email to identify your account"];

    const value = email.value;

    const errors = flatten([
        !includes(value, "@") ? ["missing an '@'"] : [],
        !includes(value, ".") ? ["missing a '.'"] : [],
        value.length < 5 ? ["less than 5 characters"] : []
    ]);

    if (errors.length > 0)
        return [`That's not an email (it's ${errors[0]})`];

    return [];
}

export function validatePassword(password: Password): ValidationError[] {
    return password.value.length < 12 ? [ `Add another ${12 - password.value.length} characters` ] : [];
}

export function withKey<T extends React.ReactElement<any>>(key: React.Key, element: T) {
    return React.cloneElement(element, { key }) as T & { key: React.Key };
}

export function validate<T>(
    value: T | null,
    validator: (value: T) => ReadonlyArray<ValidationError>,
    existingErrors: ReadonlyArray<ValidationError> | undefined,
    invalidate: boolean) {

    if (value == null)
        return [];

    return invalidate || some(existingErrors) ?
        validator(value) :
        []
}