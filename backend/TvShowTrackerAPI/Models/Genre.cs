namespace TvShowTrackerAPI.Models;

/// <summary>
/// Genre entity
/// Requirement #6: Returns TV shows by genre
/// </summary>
public class Genre : BaseEntity
{
    /// <summary>
    /// Genre name (e.g., "Action", "Comedy", "Drama", "Sci-Fi")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Genre description
    /// </summary>
    public string? Description { get; set; }

    // ===========================================
    // NAVIGATION PROPERTIES (Relationships)
    // ===========================================

    /// <summary>
    /// TV shows in this genre (many-to-many relationship)
    /// </summary>
    public ICollection<TvShowGenre> TvShows { get; set; } = new List<TvShowGenre>();
}