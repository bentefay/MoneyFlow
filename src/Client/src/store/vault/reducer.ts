import { ActionType, getType } from "typesafe-actions";
import * as actions from "./actions";

export type VaultAction = ActionType<typeof actions>;

export type VaultState = Readonly<{
  identity: {
    email: string
  }
} | null>;

const getDefaultState = (): VaultState => {
  return null;
}

export const vaultReducer = (state = getDefaultState(), action: VaultAction): VaultState => {
  switch (action.type) {
    case getType(actions.vaultRequested):
      return state;
    default:
      return state;
  }
};