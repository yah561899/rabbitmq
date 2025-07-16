using System;

namespace Ally.RabbitMq.MessageQueue.Exceptions
{
    public class AllyRabbitMQException : AllyBaseException
    {
        public AllyRabbitMQException(
            ErrorDetail errorDetail,
            Exception? ex = null,
            object? errorObject = null)
            : base(errorDetail, ex)
        {
        }

        public class ErrorType
        {
            public static ErrorDetail UnknownError = new ErrorDetail() { Code = "10000", Message = "Unknown Error" };
            public static ErrorDetail NotImplementedError = new ErrorDetail() { Code = "10001", Message = "Not implemented" };
        }
    }
}
