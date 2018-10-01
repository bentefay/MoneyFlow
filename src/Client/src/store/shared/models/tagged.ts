import { Omit } from '.';

export type Untagged<T> = Omit<T, "type">;

export function tagged<T extends object & { type: string }>(type: T["type"]) {
    return (value: Untagged<T>): T => Object.assign({}, value, { type: type }) as any;
}
