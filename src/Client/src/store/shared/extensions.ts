import taskEither from 'fp-ts/lib/TaskEither';
import either from 'fp-ts/lib/Either';

declare module "fp-ts/lib/Either" {
    function asLeft<L>(left: L): Either<L, never>;
    function asRight<R>(right: R): Either<never, R>;
}

either.asLeft = either.left;
either.asRight = either.right;

declare module "fp-ts/lib/TaskEither" {
    function asLeft<L>(left: L): TaskEither<L, never>;
    function asRight<R>(right: R): TaskEither<never, R>;
}

taskEither.asLeft = <L>(left: L) => taskEither.fromEither(either.left(left));
taskEither.asRight = <R>(right: R) => taskEither.fromEither(either.right(right));