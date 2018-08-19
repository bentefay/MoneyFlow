import { ActionType, getType } from "typesafe-actions";
import * as actions from "./actions";

export type AuthAction = ActionType<typeof actions>;

export type AuthState = Readonly<{
  username: string | null,
  password: string | null
}>;

const getDefaultState = (): AuthState => {
  return {
    username: null,
    password: null
  };
}

export const authReduceer = (state = getDefaultState(), action: AuthAction): AuthState => {
  switch (action.type) {
    case getType(actions.passwordUpdated):
      return { ...state, password: action.payload.password };
    case getType(actions.usernameUpdated):
      return { ...state, username: action.payload.username };
    default:
      return state;
  }
}