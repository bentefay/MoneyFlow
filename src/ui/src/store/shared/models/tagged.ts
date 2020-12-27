import { Omit } from ".";

export type RecordType<T> = {
    type: string;
} & Readonly<T>;

export function recordType<T extends RecordType<object>>(type: T["type"]) {
    return (value: Omit<T, "type">): T => {
        const newValue: T = value as any;
        newValue.type = type;
        return newValue;
    };
}

export interface TinyType<TType extends string, TValue> {
    type: TType;
    value: TValue;
}

export function tinyType<T extends TinyType<string, any>>(type: T["type"]) {
    return (value: T["value"]): T =>
        ({
            type: type,
            value: value
        } as any);
}
