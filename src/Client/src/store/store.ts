import { createStore, applyMiddleware, compose } from "redux";
import { StateType } from "typesafe-actions";
import logger from 'redux-logger';
import { rootReducer } from './reducer';
import createSagaMiddleware from 'redux-saga'
import { runSagas } from './sagas';

const sagaMiddleware = createSagaMiddleware()
const middlewares = [sagaMiddleware, logger];
const enhancer = compose(applyMiddleware(...middlewares));
const initialState = {};

export const store = createStore(rootReducer, initialState, enhancer);

sagaMiddleware.run(runSagas);

if (module.hot) {
    module.hot.accept("./reducer", () => {
        const nextRootReducer = require("./reducer");
        store.replaceReducer(nextRootReducer);
    });
}

export type RootState = StateType<typeof rootReducer>;
