import * as React from "react";
import { Email, Password } from '.';
import { includes, flatten, some } from 'lodash';
import { ValidationError } from '../shared/models';

export const minimumPasswordLength = 12;

export function validateEmail(email: Email): ValidationError[] {
    if (email.value.length == 0)
        return ["We need an email to uniquely identify your account"];

    const value = email.value;

    const errors = flatten([
        !includes(value, "@") ? ['missing an @'] : [],
        !includes(value, ".") ? ['missing a .'] : [],
        value.length < 5 ? ["less than 5 characters"] : [],
        !/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value) ? ["not of the form a@b.cd"] : []
    ]);

    if (errors.length > 0)
        return [`That's not an email (it's ${errors[0]})`];

    return [];
}

export function validatePassword(password: Password): ValidationError[] {
    return password.value.length < minimumPasswordLength ? [`Your password is not long enough`] : [];
}

export function withKey<T extends React.ReactElement<any>>(key: React.Key, element: T) {
    return React.cloneElement(element, { key }) as T & { key: React.Key };
}

export function validate<T>(
    value: T | null,
    validator: (value: T) => ReadonlyArray<ValidationError>,
    existingErrors: ReadonlyArray<ValidationError> | undefined,
    revalidate: boolean) {

    if (value == null)
        return [];

    return revalidate || some(existingErrors) ?
        validator(value) :
        []
}