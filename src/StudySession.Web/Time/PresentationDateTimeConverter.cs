using System;
using Microsoft.Extensions.Options;

namespace StudySession.Web.Time;

public class PresentationDateTimeConverter : IDateTimeConverter
{
    private readonly TimeZoneInfo _timeZone;

    public PresentationDateTimeConverter(IOptions<TimeZoneSettings> options)
    {
        string timeZoneId = options.Value.DefaultTimeZoneId;
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public DateTimeOffset ToDateTimeOffset(DateTime localDateTime)
    {
        if (localDateTime.Kind is DateTimeKind.Utc or DateTimeKind.Local)
        {
            throw new ArgumentOutOfRangeException(
                nameof(localDateTime),
                localDateTime,
                "Presentation-layer conversion expects a timezone-less local DateTime (Kind.Unspecified). This input is already tied to a clock offset and must be converted before reaching this layer.");
        }

        var unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
        return new DateTimeOffset(unspecified, _timeZone.GetUtcOffset(unspecified));
    }

    public DateTime ToLocalDateTime(DateTimeOffset dateTimeOffset)
    {
        return TimeZoneInfo.ConvertTime(dateTimeOffset, _timeZone).DateTime;
    }
}
