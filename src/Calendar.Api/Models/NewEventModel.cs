namespace Calendar.Api.Models;

/// <summary>
/// Represents a new event model.
/// </summary>
/// <param name="Subject">A subject of event.</param>
/// <param name="Description">A description of event.</param>
/// <param name="Begin">A begin of event.</param>
/// <param name="End">An end of event.</param>
public record NewEventModel(string Subject, string Description, DateTime Begin, DateTime End);