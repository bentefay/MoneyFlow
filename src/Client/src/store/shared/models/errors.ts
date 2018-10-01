import { tagged } from '.';

export interface GeneralFailure extends Readonly<{
    type: "generalFailure",
    friendly: {
        /** A present tense description of the action that failed (e.g. "Logging in", "Retrieving vault")  */
        actionDescription: string,
        reason: string
    };
    possibleSolutions?: string[];
    error: any;
}> { }

export const generalFailure = tagged<GeneralFailure>("generalFailure");

export module Errors {
    export function unexpected(actionDescription: string, error: unknown) {
        return generalFailure({
            friendly: {
                actionDescription: actionDescription,
                reason: "An unexpected error occurred"
            },
            error: error
        });
    }
}