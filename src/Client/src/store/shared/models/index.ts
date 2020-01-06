export * from "./validation";
export * from "./typeUtils";
export * from "./tagged";
export * from "./unit";
export * from "./errors";

export class EncryptedVault {
    public type = EncryptedVault;
    constructor(public readonly userId: UserId, public readonly eTag: ETag, public readonly content: string) {}
}

export class NewVaultPlaceholder {
    public type = NewVaultPlaceholder;
    constructor(public readonly userId: UserId) {}
}

export class UserId {
    public type = UserId;
    constructor(public readonly value: string) {}
}

export class ETag {
    public type = ETag;
    constructor(public readonly value: string) {}
}
