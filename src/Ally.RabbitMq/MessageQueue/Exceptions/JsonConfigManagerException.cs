using System;

namespace Ally.RabbitMq.MessageQueue.Exceptions
{
    public class JsonConfigManagerException : AllyBaseException
    {
        public JsonConfigManagerException(
            ErrorDetail errorDetail,
            Exception? ex = null,
            object? errorObject = null)
            : base(errorDetail, ex)
        {
        }

        public class ErrorType
        {
            public static ErrorDetail DeserializeError = new ErrorDetail() { Code = "0101", Message = "Json deserialize error" };
        }
    }
}
