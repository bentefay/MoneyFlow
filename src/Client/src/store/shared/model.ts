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