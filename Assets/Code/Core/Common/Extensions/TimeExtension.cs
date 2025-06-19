using System;

namespace Core.Extensions
{
    public static class TimeExtension
    {
        public static bool IsEqualDay(this DateTime lastVisit, DateTime currenVisit)
        {
            return lastVisit != DateTime.MinValue && currenVisit != DateTime.MinValue &&
                   lastVisit.Day == currenVisit.Day;
        }
    }
}