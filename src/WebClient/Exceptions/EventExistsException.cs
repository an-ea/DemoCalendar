namespace WebClient.Exceptions;

/// <summary>
/// Represents an error that happens when an attempt to create the event failed because the equal event already exists.
/// </summary>
public class EventExistsException : Exception
{
    public EventExistsException(string message) : base(message)
    {
    }
}