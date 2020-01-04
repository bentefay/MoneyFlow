
import { combineReducers } from "redux";
import { routerReducer } from "react-router-redux";
import { authReducer } from './auth/reducer';

export const rootReducer = combineReducers({
    router: routerReducer,
    auth: authReducer
});