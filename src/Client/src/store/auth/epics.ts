import { Observable, from, of } from 'rxjs';
import { mergeMap, map, filter, withLatestFrom, catchError } from 'rxjs/operators';
import { ajax } from 'rxjs/ajax';
import { authActions, AuthAction, Failure, GeneralFailure } from '.';
import { isActionOf, action } from 'typesafe-actions';
import { RootState } from '../store';
import { loginOrCreateCompleted, loginOrCreateErrored } from './actions';
import { hash } from "bcryptjs";
import { lowerFirst } from "lodash";
import * as t from 'io-ts';
import { PathReporter } from 'io-ts/lib/PathReporter';
import { left } from 'fp-ts/lib/Either';

interface Auth {
  username: string;
  password: string;
}

export function getVault(action: Observable<AuthAction>, state: Observable<RootState>) {
  return action.pipe(
    filter(isActionOf(authActions.loginOrCreateInitiated)),
    withLatestFrom(state),
    map(([_, { auth }]) => auth),
    mergeMap(auth => {
      from()

    })ajax.getJSON(`/api/vault`, toHeaders(auth.username!, hashedPassword))
    mergeMap(auth =>
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



const GetSaltSuccessResponse = t.type({
  salt: t.string
});

const GetSaltValidationErrorResponse = t.type({
  validationErrors: t.type({
    username: t.array(t.string)
  })
});

const GetSaltErrorResponse = t.type({
  message: t.string
});

export class Salt {
  constructor(public readonly value: string) { }
}

async function getVault(username: string, password: string) {

  const actionDescription = "Logging you in";
  try {
    const response = await fetch(`/api/salt`, { headers: toUsernameAuthHeaders(username) });
    if (response.headers.get("content-type") === "application/json")
      if (response.status == 200) {
        return GetSaltSuccessResponse
          .decode(response.json())
          .bimap(
            mapTypeError(actionDescription),
            ({ salt }) => new Salt(salt));
      } else if (response.status == 404) {
        return GetSaltValidationErrorResponse
          .decode(response.json())
          .bimap(
            mapTypeError(actionDescription),
            ({ salt }) => salt);
      }
  } catch (error) {
    return mapFetchError(error, actionDescription).map(loginOrCreateErrored);
  }
}

function mapTypeError(actionDescription: string) {
  return (validationErrors: t.ValidationError[]): GeneralFailure => {
    const errors = PathReporter.report(left(validationErrors))
    return {
      friendly: { actionDescription: actionDescription, reason: "Our server returned an invalid response or someone else's server responded" },
      error: errors
    };
  };
}

function mapFetchError(error: unknown, actionDescription: string): GeneralFailure[] {
  if (error instanceof TypeError) {
    return [{
      friendly: { actionDescription: actionDescription, reason: "There is something wrong with your internet connection or our server is down" },
      error: error,
      possibleSolutions: ["Are you connected to the internet?", "If our server is down, then there's not much you can do but keep trying until we come back online."]
    }];
  } else if (error instanceof DOMException && error.name === "AbortError") {
    return [];
  } else {
    return [{
      friendly: { actionDescription: actionDescription, reason: "An unknown error occurred while trying to connect to our server." },
      error: error,
      possibleSolutions: ["Are you connected to the internet?", "If our server is down, then there's not much you can do but keep trying until we come back online."]
    }];
  }
}

function toHash(username: string, password: string): Promise<string> {
  return hash(password, username);
}

function toUsernameAuthHeaders(username: string) {
  return toAuthHeader({ username });
}

type HashedPassword = string;

function toAuthHeaders(username: string, password: HashedPassword) {
  return toAuthHeader({ username, password });
}

function toAuthHeader(obj: any) {
  return { "Authorization": `Basic ${toBase64(obj)}` };
}

function toBase64(obj: any) {
  return btoa(JSON.stringify(obj))
}