using System;
using System.Collections.Generic;

namespace Ally.RabbitMq.MessageQueue.Unit
{
    public static class EnvironmentVariableReader<TEnum>
        where TEnum : Enum
    {
        private static readonly Dictionary<TEnum, (string Value, DateTime LastUpdateTime)> EnvironmentVariablesCache =
            new Dictionary<TEnum, (string, DateTime)>();

        private static readonly int CacheDurationInSeconds = 60;

        static EnvironmentVariableReader()
        {
            InitializeEnvironmentVariablesCache();
        }

        public static string Get(TEnum variable)
        {
            if (IsValueCached(variable, out var cachedValue))
            {
                return cachedValue;
            }

            string variableName = variable.ToString();
            string value = GetEnvironmentVariableValue(variableName);
            UpdateEnvironmentVariableCache(variable, value);

            return value;
        }

        private static void InitializeEnvironmentVariablesCache()
        {
            foreach (TEnum variable in Enum.GetValues(typeof(TEnum)))
            {
                Get(variable);
            }
        }

        private static bool IsValueCached(TEnum variable, out string cachedValue)
        {
            if (EnvironmentVariablesCache.TryGetValue(variable, out var cachedData))
            {
                if ((DateTime.Now - cachedData.LastUpdateTime).TotalSeconds < CacheDurationInSeconds)
                {
                    cachedValue = cachedData.Value;
                    return true;
                }
            }

            cachedValue = null;
            return false;
        }

        private static string GetEnvironmentVariableValue(string variableName)
        {
            string value = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Process);
            if (value == null)
            {
                //throw new Exception($"環境變數 '{variableName}' 未找到，且未設定預設值。");
            }
            return value;
        }

        private static void UpdateEnvironmentVariableCache(TEnum variable, string value)
        {
            EnvironmentVariablesCache[variable] = (value, DateTime.Now);
        }
    }
}
