using System;

namespace Dx29.Web.Services
{
    partial class LocalizationService
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 24 * HOUR;
        const int MONTH = 30 * DAY;

        public string AsElapsedTime(DateTimeOffset date)
        {
            var ts = new TimeSpan(DateTimeOffset.UtcNow.Ticks - date.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return Localize("Now");

            if (delta < 2 * MINUTE)
                return Localize("a minute ago");

            if (delta < 45 * MINUTE)
                return Localize("{0} minutes ago", ts.Minutes);

            if (delta < 90 * MINUTE)
                return Localize("an hour ago");

            if (delta < 24 * HOUR)
                return Localize("{0} hours ago", ts.Hours);

            if (delta < 48 * HOUR)
                return Localize("yesterday");

            if (delta < 30 * DAY)
                return Localize("{0} days ago", ts.Days);

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? Localize("one month ago") : Localize("{0} months ago", months);
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? Localize("one year ago") : Localize("{0} years ago", years);
            }
        }

        public string AsAge(TimeSpan delta)
        {
            var y = Years(delta);
            var m = Months(delta);
            var d = Days(delta);

            if (y != null)
            {
                if (m != null)
                {
                    return Localize("{0}, {1}", y, m);
                }
                return y;
            }
            if (m != null)
            {
                if (d != null)
                {
                    return Localize("{0}, {1}", m, d);
                }
                return m;
            }
            if (d != null)
            {
                return d;
            }
            return Localize("{0} days", 0);
        }

        private string Years(TimeSpan delta)
        {
            int v = (int)(delta.Days / 365);
            return v > 1 ? Localize("{0} years", v) : v > 0 ? Localize("{0} year", v) : null;
        }

        private string Months(TimeSpan delta)
        {
            int v = (delta.Days % 365) / 30;
            return v > 1 ? Localize("{0} months", v) : v > 0 ? Localize("{0} month", v) : null;
        }

        private string Days(TimeSpan delta)
        {
            int v = (delta.Days % 30);
            return v > 1 ? Localize("{0} days", v) : v > 0 ? Localize("{0} day", v) : null;
        }
    }
}
