import { NonFunctionKeys } from '.';

export type DeepPartial<T> =
    T extends any[] ? DeepPartialArray<T[number]> :
    T extends object ? DeepPartialObject<T> :
    T

export type DeepPartialObject<T> = {
    readonly [P in NonFunctionKeys<T>]?: DeepPartial<T[P]>
}

export interface DeepPartialArray<T> extends ReadonlyArray<DeepPartial<T>> { }