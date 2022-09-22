namespace Calendar.Api.Models;

/// <summary>
/// Represents an event model.
/// </summary>
/// <param name="Id">An id of event.</param>
/// <param name="Subject">A subject of event.</param>
/// <param name="Description">A description of event.</param>
/// <param name="Begin">A begin of event.</param>
/// <param name="End">An end of event.</param>
public record EventModel(int Id, string Subject, string Description, DateTime Begin, DateTime End)
    : NewEventModel(Subject, Description, Begin, End);