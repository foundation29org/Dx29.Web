using System;

namespace Dx29.Web.Services
{
    partial class LocalizationService
    {
        const string NULL_DATE_STR = "-";

        public string AsAge(DateTimeOffset? date)
        {
            if (date != null)
            {
                return AsAge(date.Value);
            }
            return Localize("Unknown");
        }
        public string AsAge(DateTimeOffset date)
        {
            var now = DateTimeOffset.UtcNow;
            return now > date ? AsAge(DateTimeOffset.UtcNow - date) : $"- {AsAge(date - DateTimeOffset.UtcNow)}";
        }

        public string AsElapsedTime(DateTimeOffset? date)
        {
            if (date != null)
            {
                return AsElapsedTime(date.Value);
            }
            return NULL_DATE_STR;
        }

        public string AsShortDateTime(DateTimeOffset? date)
        {
            if (date != null)
            {
                return AsShortDateTime(date.Value);
            }
            return NULL_DATE_STR;
        }
        public string AsShortDateTime(DateTimeOffset date)
        {
            date = date.ToLocalTime();
            return $"{date.DateTime.ToShortDateString()} {date.DateTime.ToShortTimeString()}";
        }

        public string AsShortDate(DateTimeOffset? date)
        {
            if (date != null)
            {
                return AsShortDate(date.Value);
            }
            return NULL_DATE_STR;
        }
        public string AsShortDate(DateTimeOffset date)
        {
            date = date.ToLocalTime();
            return $"{date.DateTime.ToShortDateString()}";
        }

        public string ToShortDateString(DateTimeOffset? date)
        {
            if (date != null)
            {
                return date.Value.DateTime.ToLocalTime().ToShortDateString();
            }
            return NULL_DATE_STR;
        }

        public string AsFileSize(long? size)
        {
            return AsFileSize(size ?? 0);
        }
        public string AsFileSize(long size)
        {
            return AsFileSize((double)size);
        }
        public string AsFileSize(double size)
        {
            if (size < 1024) return $"{size:0} B";
            if (size < 1024 * 1024) return $"{(size / 1024):0.##} Kb";
            if (size < 1024 * 1024 * 1024) return $"{(size / (1024 * 1024)):0.##} Mb";
            return $"{(size / (1024 * 1024 * 1024)):0.##} Gb";
        }
    }
}
