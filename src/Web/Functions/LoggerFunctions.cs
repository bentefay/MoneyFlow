using System;
using System.Runtime.CompilerServices;
using Serilog;
using Web.Types.Errors;

namespace Web.Functions
{
    public static class LoggerFunctions {

        public static Action<IError> LogControllerError(ILogger logger)
        {
            return error => {
                switch (logger)
                {
                    case IErrorWithException e:
                        logger.Warning(e.Exception, "Error in controller action: {Description}", error.GetDescription());
                        return;
                    default:
                        logger.Warning("Error in controller action: {Description}", error.GetDescription());
                        return;
                }
            };
        }

    }
}