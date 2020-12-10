using System;


namespace Tradnitro.Shared.Utilities
{
    public static class DateTimeUtilityExtensions
    {
        public static string ToUniversalString(this DateTime value) =>
            DateTime.SpecifyKind(value, DateTimeKind.Utc).ToString("O");


        public static string ToUniversalString(this DateTime? value) =>
            value.HasValue ? value.Value.ToUniversalString() : "Unspecified";
    }
}