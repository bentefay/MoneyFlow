import { login } from './auth';
import { all } from 'redux-saga/effects';

export function* runSagas() {
    yield all([
        login()
    ]);
}