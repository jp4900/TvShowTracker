namespace TvShowTrackerAPI.Models;

/// <summary>
/// Actor entity
/// </summary>
public class Actor : BaseEntity
{
    /// <summary>
    /// Actor's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Actor's biography
    /// </summary>
    public string? Biography { get; set; }

    /// <summary>
    /// Actor's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Actor's place of birth
    /// </summary>
    public string? PlaceOfBirth { get; set; }

    /// <summary>
    /// URL to actor's profile photo
    /// </summary>
    public string? ProfilePath { get; set; }

    /// <summary>
    /// Popularity score (for sorting)
    /// </summary>
    public decimal? Popularity { get; set; }

    /// <summary>
    /// Gender of the actor (optional)
    /// 0 = Not specified, 1 = Female, 2 = Male, 3 = Non-binary
    /// </summary>
    public int? Gender { get; set; }

    // ===========================================
    // NAVIGATION PROPERTIES (Relationships)
    // ===========================================

    /// <summary>
    /// TV shows this actor has appeared in (many-to-many)
    /// </summary>
    public ICollection<TvShowActor> TvShows { get; set; } = new List<TvShowActor>();
}