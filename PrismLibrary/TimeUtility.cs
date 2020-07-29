using System;

namespace PrismLibrary
{
    internal static class TimeUtility
    {
        /// <summary>
        /// Converts the specified time in seconds since 1970-01-01 00:00:00 to DateTime.
        /// </summary>
        /// <param name="epochTime"></param>
        /// <returns></returns>
        public static DateTime EpochToDateTime(long epochTime)
        {
            return DateTime.UnixEpoch.AddSeconds(epochTime);
        }
    }
}