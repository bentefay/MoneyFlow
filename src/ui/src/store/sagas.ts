import { signIn } from "./sagas/signIn";
import { all } from "redux-saga/effects";

export function* runSagas() {
    yield all([signIn()]);
}
