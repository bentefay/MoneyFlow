import { getType } from "typesafe-actions";
import { AuthAction, AuthState, authActions } from '.';
import { withValue } from '../shared/functions';
import { validatePassword, validateEmail, validate } from './validation';
import { Email, Password } from './model';

const getDefaultState = (): AuthState => {
  return {
    value: {
      email: null,
      password: null
    },
    errors: {}
  };
}

export const authReducer = (state = getDefaultState(), action: AuthAction): AuthState => {
  switch (action.type) {
    case getType(authActions.emailUpdated):
      return withValue(state, {
        value: { email: action.payload.email },
        errors: { email: validate(action.payload.email, validateEmail, state.errors.email, action.payload.invalidate) }
      });
    case getType(authActions.passwordUpdated):
      return withValue(state, {
        value: { password: action.payload.password },
        errors: { password: validate(action.payload.password, validatePassword, state.errors.password, action.payload.invalidate) }
      });
    case getType(authActions.loginErrored):
      return action.payload.type == "generalFailure" ?
        withValue(state, { generalFailure: action.payload }) :
        withValue(state, { errors: action.payload.errors });
    case getType(authActions.loginInitiated):
      return withValue(state, {
        generalFailure: undefined,
        errors: {
          email: validateEmail(state.value.email || new Email("")),
          password: validatePassword(state.value.password || new Password("")),
        }
      });
    default:
      return state;
  }
}