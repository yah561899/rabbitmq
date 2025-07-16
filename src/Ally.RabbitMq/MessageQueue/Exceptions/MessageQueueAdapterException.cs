using System;

namespace Ally.RabbitMq.MessageQueue.Exceptions
{
    public class MessageQueueConfigAdapterException : AllyBaseException
    {
        public MessageQueueConfigAdapterException(
            ErrorDetail errorDetail,
            Exception? ex = null,
            object? errorObject = null)
            : base(errorDetail, ex)
        {
        }

        public class ErrorType
        {
            public static ErrorDetail InitializeError = new ErrorDetail() { Code = "0101", Message = "MQAdapter has initial error." };
            public static ErrorDetail RegisterReceivedError = new ErrorDetail() { Code = "0201", Message = "Register method event error." };
            public static ErrorDetail DoReceivedError = new ErrorDetail() { Code = "0202", Message = "DoReceivedError" };
            public static ErrorDetail ReceivedOverTimesError = new ErrorDetail() { Code = "0203", Message = "ReceivedOverTimesError" };
            public static ErrorDetail PushMessageError = new ErrorDetail() { Code = "0301", Message = "MQAdapter push message with priority error." };
            public static ErrorDetail PublishError = new ErrorDetail() { Code = "0302", Message = "Register method event error." };
        }
    }
}