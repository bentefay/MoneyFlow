import { NonFunctionKeys } from ".";
import { ReactElement, Key } from "react";
import _ from "lodash";
import React from "react";

export type FormError = string | (ReactElement<any> & { key: Key });
export type FormField<T> = { value: T; errors: FormError[]; touched: boolean };

export type FormErrors<T> = T extends (infer A)[] ? ArrayFormErrors<A> : T extends object ? ObjectFormErrors<T> : ReadonlyArray<FormError>;

export type ObjectFormErrors<T> = { readonly [P in NonFunctionKeys<T>]?: FormErrors<T[P]> };

export interface ArrayFormErrors<T> extends ReadonlyArray<FormErrors<T>> {}

export type FormStateValidator<T> = (state: FormState<T>) => ObjectFormErrors<T>;

export type FormState<T> = { readonly [P in NonFunctionKeys<T>]: FormField<T[P]> };

export type SetState<T> = (updateState: (state: T) => T) => void;

type Writable<T> = {
    -readonly [K in keyof T]: T[K];
};

export const formState = <T>(value: FormState<T>): FormState<T> => value;
export const formField = <T>(value: T): FormField<T> => ({ value: value, errors: [], touched: false });

const keys = <T>(object: T) => {
    return (Object.keys(object) as any) as ReadonlyArray<keyof T>;
};

const values = <T>(object: T) => {
    return keys(object).map(key => object[key]) as ReadonlyArray<T[keyof T]>;
};

export const useFormState = <TState>(defaultState: () => FormState<TState>, validator: FormStateValidator<TState>, onValid: (state: TState) => void) => {
    const [state, setState] = React.useState(defaultState);
    return {
        onChange: onFormStateChange(setState, validator),
        onSubmit: onFormStateSubmit(setState, validator, onValid),
        isValid: isValid(state),
        state,
        reset: (type: "all" | "errors" = "all") => (type == "all" ? setState(defaultState()) : setState(clearErrors(state)))
    };
};

const validateState = <TState>(state: Writable<FormState<TState>>, changeType: "blur" | "change", validator: FormStateValidator<TState>) => {
    const stateErrors = validator(state);
    _.forEach(keys(stateErrors), key => {
        const revalidate = changeType == "blur" || state[key].errors.length > 0;
        if (revalidate) {
            const errors = stateErrors[key];
            state[key] = {
                ...state[key],
                errors: errors ?? []
            };
        }
    });
};

export type ChangeType = "blur" | "change";
export type EventWithValue<TValue> = { currentTarget: { value: TValue } };
export type OnChange<TState, TKey extends keyof FormState<TState>> = (key: TKey, type: ChangeType) => (event: EventWithValue<TState[TKey]>) => void;

export const onFormStateChange = <TState>(setState: SetState<FormState<TState>>, validator: FormStateValidator<TState>) => {
    return <TKey extends NonFunctionKeys<TState>>(key: TKey, changeType: ChangeType) => {
        return <TValue extends TState[TKey]>(event: EventWithValue<TValue>) => {
            const newValue = event.currentTarget.value;
            setState(state => {
                const field = state[key];
                const newState = {
                    ...state,
                    [key]: {
                        ...field,
                        value: newValue,
                        touched: field.touched || (newValue != null && (newValue as any) != "")
                    }
                } as Writable<typeof state>;

                validateState(newState, changeType, validator);

                return newState;
            });
        };
    };
};

export const onFormStateSubmit = <TState>(setState: SetState<FormState<TState>>, validator: FormStateValidator<TState>, onValid: (state: TState) => void) => {
    return () => {
        setState(state => {
            const newState = {
                ...state
            } as Writable<typeof state>;

            _.forEach(keys(newState), key => {
                newState[key] = {
                    ...state[key],
                    touched: true
                };
            });

            validateState(newState, "blur", validator);

            if (isValid(newState)) {
                onValid(extractState(newState));
            }

            return newState;
        });
    };
};

export const isValid = <TState>(state: FormState<TState>) => {
    return _(values(state)).every(field => field.errors.length == 0);
};

export const clearErrors = <TState>(formState: FormState<TState>): FormState<TState> => {
    const newFormState = {} as Writable<FormState<TState>>;
    _.forEach(keys(formState), key => {
        newFormState[key] = formField(formState[key].value);
    });
    return newFormState;
};

export const extractState = <TState>(formState: FormState<TState>): TState => {
    const state = {} as Writable<TState>;
    _.forEach(keys(formState), key => {
        state[key] = formState[key].value;
    });
    return state;
};
