namespace Calendar.Domain;

/// <summary>
/// Represents a result of performing an event updating.
/// </summary>
public enum ResultOfEventUpdating
{
    /// <summary>
    /// An event has been updated. 
    /// </summary>
    Success,

    /// <summary>
    /// An event hasn't been found.
    /// </summary>
    NotFound,

    /// <summary>
    /// An event can't be updated because there is the equal event.
    /// </summary>
    AlreadyExists
}