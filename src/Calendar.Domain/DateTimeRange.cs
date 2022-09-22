namespace Calendar.Domain;

/// <summary>
/// Represents a range between two <see cref="DateTime" />s.
/// </summary>
public readonly struct DateTimeRange
{
    /// <summary>
    /// Initializes a <see cref="DateTimeRange" /> from a given <paramref name="begin"/> and <paramref name="end"/>.
    /// </summary>
    /// <param name="begin">Begin of a range.</param>
    /// <param name="end">End of a range</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public DateTimeRange(DateTime begin, DateTime end)
    {
        if (end < begin)
            throw new ArgumentOutOfRangeException($"{nameof(end)} is less than {nameof(begin)}");

        Begin = begin;
        End = end;
    }


    /// <summary>
    /// A begin of a range.
    /// </summary>
    public DateTime Begin { get; }

    /// <summary>
    /// An end of a range.
    /// </summary>
    public DateTime End { get; }
}