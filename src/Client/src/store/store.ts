import { createStore, applyMiddleware, compose } from "redux";
import { createEpicMiddleware, combineEpics } from "redux-observable";
import { combineReducers } from "redux";
import { routerReducer } from "react-router-redux";
import { StateType } from "typesafe-actions";
import { vaultReduceer } from "./vault/reducer";
import { authReduceer } from './auth/reducer';
import logger from 'redux-logger';

const rootEpic = combineEpics();

const rootReducer = combineReducers({
  router: routerReducer,
  vault: vaultReduceer,
  auth: authReduceer
});

const epicMiddleware = createEpicMiddleware();
const middlewares = [epicMiddleware, logger];
const enhancer = compose(applyMiddleware(...middlewares));
const initialState = {};

export const store = createStore(rootReducer, initialState, enhancer);

epicMiddleware.run(rootEpic)

export type RootState = StateType<typeof rootReducer>;
