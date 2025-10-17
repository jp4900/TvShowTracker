namespace TvShowTrackerAPI.Models;

/// <summary>
/// Join table for many-to-many relationship between TvShow and Actor
/// This stores additional info like character name and billing order
/// </summary>
public class TvShowActor
{
    /// <summary>
    /// Foreign key to TvShow
    /// </summary>
    public Guid TvShowId { get; set; }

    /// <summary>
    /// Navigation property to TvShow
    /// </summary>
    public TvShow TvShow { get; set; } = null!;

    /// <summary>
    /// Foreign key to Actor
    /// </summary>
    public Guid ActorId { get; set; }

    /// <summary>
    /// Navigation property to Actor
    /// </summary>
    public Actor Actor { get; set; } = null!;

    /// <summary>
    /// Name of the character the actor plays
    /// </summary>
    public string? CharacterName { get; set; }

    /// <summary>
    /// Role Type (0 = main role, 1 = second billed, etc.)
    /// Used to determine "featured" actors
    /// </summary>
    public int? Order { get; set; }
}