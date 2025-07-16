using Newtonsoft.Json;
using System;

namespace Ally.RabbitMq.MessageQueue.Models
{
    public class DataModel
    {
        [JsonProperty("traceId")]
        public Guid TraceId { get; set; }

        [JsonProperty("AllyTokenKey")]
        public string AllyTokenKey { get; set; }

        [JsonProperty("functionName")]
        public string FunctionName { get; set; }

        [JsonProperty("tokenKey")]
        public string TokenKey { get; set; }

        [JsonProperty("userUuid")]
        public string UserUuid { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
