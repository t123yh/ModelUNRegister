using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ModelUNRegister.Utilities
{
    public static class DateTimeConverter
    {
        public static string ToDisplayString(this DateTime val, DateTime reference)
        {
            TimeSpan duration = (reference - val).Duration();
            bool isPast = (reference - val) > TimeSpan.Zero;
            string pstr = isPast ? "前" : "后";
            if (duration < TimeSpan.FromMinutes(1))
            {
                return string.Format("{0} 秒{1}", duration.Seconds, pstr);
            }
            else if (duration < TimeSpan.FromHours(1))
            {
                return string.Format("{0} 分钟{1}", duration.Minutes, pstr);
            }
            else if (duration < TimeSpan.FromDays(1))
            {
                return string.Format("{0} 小时{1}", duration.Hours, pstr);
            }
            else if (duration < TimeSpan.FromDays(30))
            {
                return string.Format("{0} 天{1}", duration.Days, pstr);
            }
            else if(val.Year == reference.Year)
            {
                return val.ToString("M 月 dd 日");
            }
            else
            {
                return val.ToString("yyyy 年 M 月 dd 日");
            }
        }
    }
}