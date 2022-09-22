using System.ComponentModel.DataAnnotations;

namespace WebClient.Models;

/// <summary>
/// Represents an event.
/// </summary>
public class Event : NewEvent
{
    /// <summary>
    /// An id of event.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int EventId { get; init; }
}