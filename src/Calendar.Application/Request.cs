namespace Calendar.Application;

/// <summary>
/// Represents an abstract request.
/// </summary>
public abstract class Request
{
    /// <summary>
    /// A user id.
    /// </summary>
    public int UserId { get; init; }
}