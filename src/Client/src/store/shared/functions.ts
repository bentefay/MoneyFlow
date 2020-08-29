export function valueOrDefault<T>(tinyType: { value: T } | null, defaultValue: T): T {
    return tinyType == null ? defaultValue : tinyType.value;
}
