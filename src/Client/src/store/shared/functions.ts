import { DeepPartial } from './models/deepPartial';
import { mergeWith } from 'lodash';
import { isArray } from 'util';

export function valueOrDefault<T>(tinyType: { value: T } | null, defaultValue: T): T {
    return tinyType == null ? defaultValue : tinyType.value;
}

export function withValue<T>(existing: T, overrides: DeepPartial<T>): T {
    return mergeWith({}, existing, overrides,
        (existing, overrides) =>
            isArray(existing) && isArray(overrides) ?
                overrides :
                undefined);
}