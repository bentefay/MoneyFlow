using System;
using Microsoft.Azure.Storage;

namespace Web.Types.Errors
{
    internal class CloudStorageError : IError
    {
        private readonly Exception _exception;
        private readonly string _action;

        public CloudStorageError(Exception exception, string action)
        {
            _exception = exception;
            _action = action;
        }

        public string GetDescription()
        {
            var message = _exception is StorageException e ?
                $"{e.Message}\n\nRequest information\n" +
                $"ETag: {e.RequestInformation.Etag}\n" +
                $"ErrorCode: {e.RequestInformation.ErrorCode}\n" +
                $"StartTime: {e.RequestInformation.StartTime}\n" +
                $"EndTime: {e.RequestInformation.EndTime}\n" +
                $"ExtendedErrorCode: {e.RequestInformation.ExtendedErrorInformation.ErrorCode}\n" +
                $"ExtendedErrorMessage: {e.RequestInformation.ExtendedErrorInformation.ErrorMessage}\n" +
                $"HttpStatusCode: {e.RequestInformation.HttpStatusCode}\n" +
                $"HttpStatusMessage: {e.RequestInformation.HttpStatusMessage}\n" :
                _exception.Message;

            return $"Error while {_action}: {message}";
        }
    }

    internal class BlobAlreadyExistsError : CloudStorageError
    {
        public BlobAlreadyExistsError(StorageException exception, string action) : base(exception, action)
        {
        }
    }

    internal class BlobETagIncorrect : CloudStorageError
    {
        public BlobETagIncorrect(StorageException exception, string action) : base(exception, action)
        {
        }
    }
}