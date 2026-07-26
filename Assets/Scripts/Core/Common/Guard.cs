using System;

namespace SReader.Core.Common
{
    /// <summary>Argument validation helpers for constructors (DI) and services.</summary>
    public static class Guard
    {
        public static T NotNull<T>(T value, string paramName) where T : class
        {
            if (value == null) throw new ArgumentNullException(paramName);
            return value;
        }

        public static string NotNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{paramName}' must not be null or empty.", paramName);
            return value;
        }
    }
}
