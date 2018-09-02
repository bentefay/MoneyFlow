import { createStore, applyMiddleware, compose } from "redux";
import { createEpicMiddleware, combineEpics } from "redux-observable";
import { combineReducers } from "redux";
import { routerReducer } from "react-router-redux";
import { StateType } from "typesafe-actions";
import { vaultReducer } from "./vault/reducer";
import { authReducer } from './auth/reducer';
import logger from 'redux-logger';
import { getVault } from './auth';

const rootEpic = combineEpics(getVault);

const rootReducer = combineReducers({
  router: routerReducer,
  vault: vaultReducer,
  auth: authReducer
});

const epicMiddleware = createEpicMiddleware();
const middlewares = [epicMiddleware, logger];
const enhancer = compose(applyMiddleware(...middlewares));
const initialState = {};

export const store = createStore(rootReducer, initialState, enhancer);

epicMiddleware.run(rootEpic as any)

export type RootState = StateType<typeof rootReducer>;
