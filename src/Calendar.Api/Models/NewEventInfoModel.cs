namespace Calendar.Api.Models;

/// <summary>
/// Represents an information about new event.
/// </summary>
/// <param name="Id">An id of a new event.</param>
public readonly record struct NewEventInfoModel(int Id);