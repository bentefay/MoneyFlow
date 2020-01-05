import { recordType, RecordType } from ".";

export interface GeneralError
    extends RecordType<{
        type: "GeneralError";
        friendly: {
            /** A present tense description of the action that failed (e.g. "Logging in", "Retrieving vault")  */
            actionDescription: string;
            reason: string;
        };
        possibleSolutions?: string[];
        error: any;
    }> {}

export const newGeneralError = recordType<GeneralError>("GeneralError");

export module Errors {
    export function unexpected(actionDescription: string, error: unknown) {
        return newGeneralError({
            friendly: {
                actionDescription: actionDescription,
                reason: "An unexpected error occurred"
            },
            error: error
        });
    }
}
