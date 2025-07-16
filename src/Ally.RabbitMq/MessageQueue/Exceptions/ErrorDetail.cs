namespace Ally.RabbitMq.MessageQueue.Exceptions
{
    public class ErrorDetail
    {
        public string Code { get; set; }

        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public string Prefix { get; set; }

        public string ErrorLevel { get; set; } = CommonUtilConstants.ErrorLevelP4;
    }
}
