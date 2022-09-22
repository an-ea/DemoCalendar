namespace Calendar.Domain;

/// <summary>
/// Represents a result of performing an event creating.
/// </summary>
/// <param name="AlreadyExists"><see langword="true"/> - an event has not been created because it's already exist, <see langword="false"/> - an event has been created.</param>
/// <param name="Id">An id of created event.</param>
public readonly record struct ResultOfEventCreating(bool AlreadyExists, int Id);
