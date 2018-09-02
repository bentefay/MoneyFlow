import { Observable, from, of } from 'rxjs';
import { mergeMap, map, filter, withLatestFrom, catchError } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import { authActions, AuthAction } from '.';
import { isActionOf } from 'typesafe-actions';
import { RootState } from '../store';
import { loginOrCreateCompleted } from './actions';
import { hash } from "bcryptjs";

export function getVault(action: Observable<AuthAction>, state: Observable<RootState>) {
  return action.pipe(
    filter(isActionOf(authActions.loginOrCreateInitiated)),
    withLatestFrom(state),
    mergeMap(([_, { auth }]) =>
      from(toHash(auth.username!, auth.password!))
        .pipe(
          map(hashedPassword => ajax.getJSON(`/api/vault`, toHeaders(auth.username!, hashedPassword))),
          map(response => loginOrCreateCompleted()),
          catchError(error => {
            console.log("Something failed", error)
            return of(loginOrCreateCompleted());
          })
        )
    ));
}

function toHash(username: string, password: string): Promise<string> {
  return hash(password, username);
}

function toHeaders(username: string, hashedPassword: string) {
  return { "Authorization": `Basic ${toAuthorizationHeader(username, hashedPassword)}` };
}

function toAuthorizationHeader(username: string, hashedPassword: string) {
  return btoa(JSON.stringify({ username: username, password: hashedPassword }))
}