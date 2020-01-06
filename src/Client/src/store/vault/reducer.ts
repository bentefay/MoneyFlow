import { getType } from "typesafe-actions";
import { VaultActions, vaultActions } from "./actions";
import { VaultState } from "./model";

const getDefaultState = (): VaultState => {
    return {};
};

export const vaultReducer = (state = getDefaultState(), action: VaultActions): VaultState => {
    switch (action.type) {
        case getType(vaultActions.vaultReceived):
            return state;

        default:
            return state;
    }
};
