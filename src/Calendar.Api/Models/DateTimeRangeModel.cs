namespace Calendar.Api.Models;

/// <summary>
/// Represents a range of date times.
/// </summary>
/// <param name="Begin">A begin of a range.</param>
/// <param name="End">An end of a range.</param>
public record DateTimeRangeModel(DateTime Begin, DateTime End);