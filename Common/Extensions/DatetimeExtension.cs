using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Extensions
{
    public static class DatetimeExtension
    {
        public static DateTimeOffset ToDateTimeOffSet(this DateTime datetime)
        {
            var localTime1 = DateTime.SpecifyKind(datetime, DateTimeKind.Local);
            DateTimeOffset resultOffset = DateTimeOffset.FromFileTime(datetime.ToFileTimeUtc());
            return resultOffset;
        }
    }
}
