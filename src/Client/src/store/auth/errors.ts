import { recordType, RecordType } from "../shared/models";

export interface CredentialsIncorrectError
    extends RecordType<{
        type: "CredentialsIncorrectError";
        description: string;
    }> {}

export const newCredentialsIncorrectError = recordType<CredentialsIncorrectError>("CredentialsIncorrectError");

export interface AccountAlreadyExistsError
    extends RecordType<{
        type: "AccountAlreadyExists";
        description: string;
    }> {}

export const newAccountAlreadyExists = recordType<AccountAlreadyExistsError>("AccountAlreadyExists");

export module Errors {
    export module Auth {
        export function credentialsIncorrect(description: string) {
            return newCredentialsIncorrectError({ description: description });
        }

        export function accountAlreadyExists(description: string) {
            return newAccountAlreadyExists({ description: description });
        }
    }
}
