using System;
using System.Text.Json;

namespace Ally.RabbitMq.MessageQueue.Exceptions
{
    /// <summary>
    /// AllyException
    /// </summary>
    public abstract class AllyBaseException : Exception
    {
        public ErrorDetail Detail { get; set; }

        public object? ErrorObject { get; set; }

        protected AllyBaseException(
            (string Code, string Message) errorDetail,
            Exception? ex = null,
            object? errorObject = null)
            : base(errorDetail.Message, ex)
        {
            Detail = new ErrorDetail();
            Detail.Code = errorDetail.Code;
            Detail.Message = errorDetail.Message;
            Detail.Prefix = this.GetType().Name.Replace("Exception", string.Empty);
            Detail.ErrorCode = $"{Detail.Prefix}{errorDetail.Code}";
            ErrorObject = errorObject;
        }

        protected AllyBaseException(
            ErrorDetail errorDetail,
            Exception? ex = null,
            object? errorObject = null)
            : base(errorDetail.Message, ex)
        {
            Detail = errorDetail;
            Detail.Prefix = this.GetType().Name.Replace("Exception", string.Empty);
            Detail.ErrorCode = $"{Detail.Prefix}{errorDetail.Code}";
            ErrorObject = errorObject;
        }

        public override string ToString()
        {
            if (this.InnerException == null)
            {
                return string.Format($"{Detail.ErrorLevel} {Detail.ErrorCode} {this.Message}. [StackTrace] {this.StackTrace}, [ErrorData] {JsonSerializer.Serialize(ErrorObject)}");
            }

            return string.Format($"{Detail.ErrorLevel} {Detail.ErrorCode} {this.Message}. [InnerException] {this.InnerException}, [StackTrace] {this.StackTrace}, [ErrorData] {JsonSerializer.Serialize(ErrorObject)}");
        }
    }
}
