using System;
using Microsoft.Azure.Storage;
using Web.Functions;

namespace Web.Types.Errors
{
    public class StorageError : IError, IErrorWithException
    {
        public Exception Exception { get; }
        private readonly string _action;

        protected StorageError(Exception exception, string action)
        {
            Exception = exception;
            _action = action;
        }

        public string GetDescription()
        {
            var message = Exception is StorageException e ?
                $"{e.Message}\n\nRequest information\n" +
                $"ETag: {e.RequestInformation.Etag}\n" +
                $"ErrorCode: {e.RequestInformation.ErrorCode}\n" +
                $"StartTime: {e.RequestInformation.StartTime}\n" +
                $"EndTime: {e.RequestInformation.EndTime}\n" +
                $"ExtendedErrorCode: {e.RequestInformation.ExtendedErrorInformation.ErrorCode}\n" +
                $"ExtendedErrorMessage: {e.RequestInformation.ExtendedErrorInformation.ErrorMessage}\n" +
                $"HttpStatusCode: {e.RequestInformation.HttpStatusCode}\n" +
                $"HttpStatusMessage: {e.RequestInformation.HttpStatusMessage}\n" :
                Exception.Message;

            return $"Error while {_action}: {message}";
        }
    }

    public class GeneralStorageError : StorageError, ISetBlobTextErrors, IGetBlobTextErrors, IGetBlobErrors
    {
        public GeneralStorageError(Exception exception, string action) : base(exception, action)
        {
        }
    }

    public class CouldNotCreateBlobBecauseItAlreadyExistsError : StorageError, ISetBlobTextErrors
    {
        public CouldNotCreateBlobBecauseItAlreadyExistsError(StorageException exception, string action) : base(exception, action)
        {
        }
    }

    public class CouldNotUpdateBlobBecauseTheETagHasChanged : StorageError, ISetBlobTextErrors
    {
        public CouldNotUpdateBlobBecauseTheETagHasChanged(StorageException exception, string action) : base(exception, action)
        {
        }
    }
}