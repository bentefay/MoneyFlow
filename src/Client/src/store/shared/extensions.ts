import * as taskEither from 'fp-ts/lib/TaskEither';
import * as either from 'fp-ts/lib/Either';

declare module "fp-ts/lib/Either" {
    function asLeft<L>(left: L): Either<L, never>;
    function asRight<R>(right: R): Either<never, R>;
}

(either as any).asLeft = either.left;
(either as any).asRight = either.right;

declare module "fp-ts/lib/TaskEither" {
    function asLeft<L>(left: L): TaskEither<L, never>;
    function asRight<R>(right: R): TaskEither<never, R>;
}

(taskEither as any).asLeft = <L>(left: L) => taskEither.fromEither(either.left(left));
(taskEither as any).asRight = <R>(right: R) => taskEither.fromEither(either.right(right));