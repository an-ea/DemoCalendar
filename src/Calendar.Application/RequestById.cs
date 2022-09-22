namespace Calendar.Application;

/// <summary>
/// Represents a request by id.
/// </summary>
public class RequestById : Request
{
    /// <summary>
    /// An id using for search.
    /// </summary>
    public int Id { get; init; }
}