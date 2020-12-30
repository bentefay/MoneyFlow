import { NonFunctionKeys } from ".";
import { ReactElement, Key, useEffect } from "react";
import _ from "lodash";
import React from "react";

export type FormError = string | (ReactElement<any> & { key: Key });
export type FormField<T> = { value: T; errors: FormError[]; touched: boolean };

export type FormErrors<T> = T extends (infer A)[] ? ArrayFormErrors<A> : T extends object ? ObjectFormErrors<T> : ReadonlyArray<FormError>;

export type ObjectFormErrors<T> = { readonly [P in NonFunctionKeys<T>]?: FormErrors<T[P]> };

export interface ArrayFormErrors<T> extends ReadonlyArray<FormErrors<T>> {}

export type FormStateValidator<T> = (state: FormState<T>) => ObjectFormErrors<T>;

export type FormState<T> = { readonly [P in NonFunctionKeys<T>]: FormField<T[P]> };
type WrappedFormState<T> = { submissionCount: number; state: FormState<T> };

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
    return keys(object).map((key) => object[key]) as ReadonlyArray<T[keyof T]>;
};

export const useFormState = <TState>(defaultState: () => FormState<TState>, validator: FormStateValidator<TState>, onValid: (state: TState) => void) => {
    const wrappedDefaultState: () => WrappedFormState<TState> = () => ({ submissionCount: 0, state: defaultState() });
    const [wrappedState, setWrappedState] = React.useState(wrappedDefaultState);

    const isValid = checkIsValid(wrappedState.state);

    // This effect and the associated submissionCount state are required because the onValid callback cannot be called inside
    // the callback of setWrappedState
    useEffect(() => {
        if (isValid && wrappedState.submissionCount > 0) {
            onValid(extractState(wrappedState.state));
        }
    }, [wrappedState.submissionCount]);

    return {
        onChange: onFormStateChange(setWrappedState, validator),
        onSubmit: onFormStateSubmit(setWrappedState, validator, onValid),
        isValid: isValid,
        state: wrappedState.state,
        reset: (type: "all" | "errors" = "all") => (type == "all" ? setWrappedState(wrappedDefaultState()) : setWrappedState(clearErrors)),
    };
};

const validateState = <TState>(state: Writable<FormState<TState>>, changeType: "blur" | "change", validator: FormStateValidator<TState>) => {
    const stateErrors = validator(state);
    _.forEach(keys(stateErrors), (key) => {
        const revalidate = changeType == "blur" || state[key].errors.length > 0;
        if (revalidate) {
            const errors = stateErrors[key];
            state[key] = {
                ...state[key],
                errors: errors ?? [],
            };
        }
    });
};

export type ChangeType = "blur" | "change";
export type EventWithValue<TValue> = { currentTarget: { value: TValue } };
export type OnChange<TState, TKey extends keyof FormState<TState>> = (key: TKey, type: ChangeType) => (event: EventWithValue<TState[TKey]>) => void;

const onFormStateChange = <TState>(setWrappedState: SetState<WrappedFormState<TState>>, validator: FormStateValidator<TState>) => {
    return <TKey extends NonFunctionKeys<TState>>(key: TKey, changeType: ChangeType) => {
        return <TValue extends TState[TKey]>(event: EventWithValue<TValue>) => {
            const newValue = event.currentTarget.value;
            setWrappedState((wrapper) => {
                const field = wrapper.state[key];
                const newState = {
                    ...wrapper.state,
                    [key]: {
                        ...field,
                        value: newValue,
                        touched: field.touched || (newValue != null && (newValue as any) != ""),
                    },
                } as Writable<typeof wrapper.state>;

                validateState(newState, changeType, validator);

                return { submissionCount: wrapper.submissionCount, state: newState };
            });
        };
    };
};

const onFormStateSubmit = <TState>(
    setWrappedState: SetState<WrappedFormState<TState>>,
    validator: FormStateValidator<TState>,
    onValid: (state: TState) => void
) => {
    return () => {
        setWrappedState((wrappedState) => {
            const newState = {
                ...wrappedState.state,
            } as Writable<typeof wrappedState.state>;

            _.forEach(keys(newState), (key) => {
                newState[key] = {
                    ...wrappedState.state[key],
                    touched: true,
                };
            });

            validateState(newState, "blur", validator);

            return { submissionCount: wrappedState.submissionCount + 1, state: newState };
        });
    };
};

const checkIsValid = <TState>(state: FormState<TState>) => {
    return _(values(state)).every((field) => field.errors.length == 0);
};

const clearErrors = <TState>(wrappedState: WrappedFormState<TState>): WrappedFormState<TState> => {
    const newFormState = {} as Writable<FormState<TState>>;
    _.forEach(keys(wrappedState.state), (key) => {
        newFormState[key] = formField(wrappedState.state[key].value);
    });
    return { submissionCount: wrappedState.submissionCount, state: newFormState };
};

const extractState = <TState>(formState: FormState<TState>): TState => {
    const state = {} as Writable<TState>;
    _.forEach(keys(formState), (key) => {
        state[key] = formState[key].value;
    });
    return state;
};
