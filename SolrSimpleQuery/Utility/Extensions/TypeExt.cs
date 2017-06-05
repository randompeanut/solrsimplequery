using System;

namespace SolrSimpleQuery.Utility.Extensions
{
    public static class TypeExt
    {
        public static string GetStringValue(this object value)
        {
            if (value is DateTime)
                return ((DateTime) value).ToString("yyyy-MM-ddThh:mm:ssZ");

            return value.ToString();
        }

        public static string GetObjectValue(this string value)
        {
            var escapedValue = value.Trim('"');
            return escapedValue.IndexOf(":", StringComparison.Ordinal) == -1
                ? string.Empty
                : escapedValue.Substring(escapedValue.IndexOf(":", StringComparison.Ordinal) + 1);
        }

        public static DateTime CreateDateTime(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime CreateDateTime(int year, int month, int day, int hour, int minute, int second)
        {
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }
    }
}