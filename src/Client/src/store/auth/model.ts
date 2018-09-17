export interface AuthState extends Readonly<{
    username: Username | null,
    password: Password | null
}> { }

export class Username {
    public type = Username;
    constructor(public readonly value: string) { }
}

export class Password {
    public type = Password;
    constructor(public readonly value: string) { }
}

export class EncryptedVault {
    public type = EncryptedVault;
    constructor(public readonly value: string) { }
}
