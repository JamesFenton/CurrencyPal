using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Functions
{
    public static class HelperFunctions
    {
        public static DateTimeOffset GetNearestHour(this DateTimeOffset now)
        {
            var currentHour = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.FromHours(0));
            var previousHour = currentHour.AddHours(-1);
            var nextHour = currentHour.AddHours(1);

            return new[]
            {
                previousHour,
                currentHour,
                nextHour,
            }
            .Select(t => (time: t, timeFromNow: Math.Abs((now - t).TotalMilliseconds))) // (time, TimeSpan from now)
            .OrderBy(t => t.timeFromNow) // take the time that is closest to now
            .First()
            .time;
        }
    }
}
