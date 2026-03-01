using System;
using System.Collections.Generic;

namespace MotorracingManager.Core
{
    internal static class Guard
    {
        public static string AgainstNullOrWhiteSpace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null, empty, or whitespace.", paramName);
            }

            return value;
        }

        public static T[] AgainstNullOrCopy<T>(IEnumerable<T> values, string paramName)
        {
            if (values == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return values is T[] array ? (T[])array.Clone() : new List<T>(values).ToArray();
        }

        public static Dictionary<TKey, TValue> AgainstNullOrCopyDictionary<TKey, TValue>(
            IEnumerable<KeyValuePair<TKey, TValue>> values,
            string paramName)
        {
            if (values == null)
            {
                throw new ArgumentNullException(paramName);
            }

            var copy = new Dictionary<TKey, TValue>();
            foreach (var pair in values)
            {
                copy.Add(pair.Key, pair.Value);
            }

            return copy;
        }
    }
}
