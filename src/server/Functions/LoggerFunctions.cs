using System;
using System.Runtime.CompilerServices;
using Serilog;
using Web.Types.Errors;

namespace Web.Functions
{
    public static class LoggerFunctions
    {

        public static Action<IError> LogControllerError(ILogger logger)
        {
            return error =>
            {
                switch (logger)
                {
                    case IErrorWithException e:
                        logger.Warning(e.Exception, "Error {ErrorType} in controller action with description {Description}", error.GetType().Name, error.GetDescription());
                        return;
                    default:
                        logger.Warning("Error {ErrorType} in controller action with description {Description}", error.GetType().Name, error.GetDescription());
                        return;
                }
            };
        }

    }
}