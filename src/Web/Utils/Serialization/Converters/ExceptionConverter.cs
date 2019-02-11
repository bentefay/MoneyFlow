using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Web.Utils.Serialization.Converters
{
    public class ExceptionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, CapturedExceptionBody.FromException((Exception)value));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.None)
                return null;

            var body = serializer.Deserialize<CapturedExceptionBody>(reader);

            switch (GetConversionType(objectType))
            {
                case ConversionType.Captured:
                    return body.ToException();

                case ConversionType.Lossy:
                    return MakeLossy(objectType, body);

                default:
                    throw new InvalidOperationException();
            }
        }

        public override bool CanConvert(Type objectType)
        {
            var conversion = GetConversionType(objectType);
            return conversion != ConversionType.None;
        }

        internal enum ConversionType
        {
            None,
            Captured,
            Lossy
        }

        private static ConversionType GetConversionType(Type t)
        {
            if (!typeof(Exception).IsAssignableFrom(t))
                return ConversionType.None;

            // Is it Exception or CapturedException?
            if (t.IsAssignableFrom(typeof(CapturedException)))
                return ConversionType.Captured;

            return ConversionType.Lossy;
        }

        private static object MakeLossy(Type destinationType, CapturedExceptionBody body)
        {
            try
            {
                var constructors = destinationType.GetConstructors();

                var messageAndInnerException = constructors.FirstOrDefault(c => IsConstructorOf(c, typeof(string), typeof(Exception)));
                if (messageAndInnerException != null)
                    return messageAndInnerException.Invoke(new object[] {body.Message, body.GetInnerException()});

                var justMessage = constructors.FirstOrDefault(c => IsConstructorOf(c, typeof(string)));
                if (justMessage != null)
                    return justMessage.Invoke(new object[] {body.Message});

                var defaultCtor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
                if (defaultCtor != null)
                    return defaultCtor.Invoke(null);

                // Nothing looks like an appropriate constructor
                return null;
            }
            catch
            {
                // That constructor wasn't very happy.
                return null;
            }
        }

        private static bool IsConstructorOf(ConstructorInfo ctor, params Type[] argTypes)
        {
            if (ctor == null)
                return false;

            var args = ctor.GetParameters();

            if (args.Length != argTypes.Length)
                return false;

            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].ParameterType != argTypes[i])
                    return false;
            }

            return true;
        }
    }

    public class CapturedException : Exception
    {
        public CapturedExceptionBody Body { get; }

        internal CapturedException(CapturedExceptionBody body)
            : base(body.Message, body.GetInnerException())
        {
            Body = body;
        }

        public override string Message => Body.Message;
        public override string Source => Body.Source;
        public override string StackTrace => Body.StackTrace;
    }

    public class CapturedExceptionBody
    {
        public CapturedExceptionBody(
            string exceptionTypeName,
            string message,
            string stackTrace,
            string source,
            IReadOnlyCollection<CapturedExceptionBody> innerExceptions
            )
        {
            ExceptionTypeName = exceptionTypeName;
            Message = message;
            StackTrace = stackTrace;
            Source = source;
            InnerExceptions = innerExceptions;
        }

        public string ExceptionTypeName { get; }
        public string Message { get; }
        public string StackTrace { get; }
        public string Source { get; }
        public IReadOnlyCollection<CapturedExceptionBody> InnerExceptions { get; }

        public Exception GetInnerException()
        {
            if (InnerExceptions == null || InnerExceptions.Count == 0)
                return null;

            if (InnerExceptions.Count == 1)
                return InnerExceptions.First().ToException();

            return new AggregateException(InnerExceptions.Select(x => x.ToException()));
        }

        internal CapturedException ToException() => new CapturedException(this);

        internal static CapturedExceptionBody FromException(Exception e)
        {
            if (e is CapturedException captured)
            {
                return captured.Body;
            }

            IReadOnlyCollection<CapturedExceptionBody> innerExceptions;

            if (e is AggregateException aggregate)
                innerExceptions = aggregate.InnerExceptions.Select(FromException).ToList();
            else
                innerExceptions = e.InnerException == null
                    ? null
                    : new[] {FromException(e.InnerException)};

            return new CapturedExceptionBody(
                e.GetType().FullName,
                e.Message,
                e.StackTrace,
                e.Source,
                innerExceptions
            );
        }
    }


}
