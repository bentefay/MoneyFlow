import { getType } from "typesafe-actions";
import { AuthAction, AuthState, authActions } from '.';

const getDefaultState = (): AuthState => {
  return {
    username: null,
    password: null
  };
}

export const authReducer = (state = getDefaultState(), action: AuthAction): AuthState => {
  switch (action.type) {
    case getType(authActions.passwordUpdated):
      return { ...state, password: action.payload.password };
    case getType(authActions.usernameUpdated):
      return { ...state, username: action.payload.username };
    default:
      return state;
  }
}