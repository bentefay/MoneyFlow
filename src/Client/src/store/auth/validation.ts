import { includes, flatten } from "lodash";
import { FormError } from "../shared/models";

export const minimumPasswordLength = 12;

export function validateEmail(value: string | null): FormError[] {
    if (value == null || value.length == 0) return ["An email is needed to identify your account"];

    const errors = flatten([
        !includes(value, "@") ? [`Emails should contain an @`] : [],
        !includes(value, ".") ? [`Emails should contain a .`] : [],
        value.length < 5 ? ["Emails should be longer than 5 characters"] : [],
        !/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value) ? ["Emails should look like steve@web.com"] : []
    ]);

    if (errors.length > 0) return [errors[0]];

    return [];
}

export function validatePassword(value: string | null): FormError[] {
    return value == null || value.length < minimumPasswordLength ? [`Your password is not long enough`] : [];
}
