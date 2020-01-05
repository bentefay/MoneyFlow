import { pipe } from "fp-ts/lib/pipeable";
import * as task from "fp-ts/lib/Task";
import * as either from "fp-ts/lib/Either";
import * as taskEither from "fp-ts/lib/TaskEither";
import { TaskEither } from "fp-ts/lib/TaskEither";
import { Task } from "fp-ts/lib/Task";

declare module "fp-ts/lib/TaskEither" {
    export function match<E, A, B>(onLeft: (e: E) => B, onRight: (a: A) => B): (ma: TaskEither<E, A>) => Task<B>;
}

export function match<E, A, B>(onLeft: (e: E) => B, onRight: (a: A) => B): (ma: TaskEither<E, A>) => Task<B> {
    return ma =>
        pipe(
            ma,
            task.map(innerEither => pipe(innerEither, either.fold(onLeft, onRight)))
        );
}

(taskEither as any).match = match;
