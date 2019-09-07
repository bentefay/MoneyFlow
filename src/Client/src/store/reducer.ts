
import { combineReducers } from "redux";
import { routerReducer } from "react-router-redux";
import { vaultReducer } from "./vault/reducer";
import { authReducer } from './auth/reducer';

export const rootReducer = combineReducers({
    router: routerReducer,
    vault: vaultReducer,
    auth: authReducer
});