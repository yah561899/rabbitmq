using System;

namespace Ally.RabbitMq.MessageQueue.Exceptions
{
    public class MessageQueueFactoryException : AllyRabbitMQException
    {
        public MessageQueueFactoryException(
            ErrorDetail errorDetail,
            Exception? ex = null,
            object? errorObject = null)
            : base(errorDetail, ex)
        {
        }

        public class ErrorType
        {
            public static ErrorDetail GetMQAdapterNotFoundError = new ErrorDetail() { Code = "0101", Message = "RabbitMq configuration not found or empty" };
            public static ErrorDetail GetMQAdapterDuplicateKeyError = new ErrorDetail() { Code = "0102", Message = "Duplicate key error" };
            public static ErrorDetail GetMQAdapterOtherError = new ErrorDetail() { Code = "0103", Message = "GetMQAdapterOtherError" };
        }
    }
}