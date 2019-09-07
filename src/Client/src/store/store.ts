import { createStore, applyMiddleware, compose } from "redux";
import { createEpicMiddleware, combineEpics } from "redux-observable";
import { StateType } from "typesafe-actions";
import logger from 'redux-logger';
import { login } from './auth';
import { rootReducer } from './reducer';

const rootEpic = combineEpics(login);


const epicMiddleware = createEpicMiddleware();
const middlewares = [epicMiddleware, logger];
const enhancer = compose(applyMiddleware(...middlewares));
const initialState = {};

export const store = createStore(rootReducer, initialState, enhancer);

epicMiddleware.run(rootEpic as any);

if (module.hot) {
    module.hot.accept("./reducer", () => {
        const nextRootReducer = require("./reducer");
        store.replaceReducer(nextRootReducer);
    });
}

export type RootState = StateType<typeof rootReducer>;
