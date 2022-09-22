using System.ComponentModel.DataAnnotations;

namespace WebClient.Models;

/// <summary>
/// Represents a new event.
/// </summary>
public class NewEvent
{
    /// <summary>
    /// A title of event.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Title { get; init; } = null!;

    /// <summary>
    /// A description of event.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; init; } = null!;

    /// <summary>
    /// A start of event.
    /// </summary>
    [Required]
    public DateTime Start { get; init; }

    /// <summary>
    /// An end of event.
    /// </summary>
    [Required]
    public DateTime End { get; init; }
}