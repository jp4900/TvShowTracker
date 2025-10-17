namespace TvShowTrackerAPI.Models;

/// <summary>
/// Join table for many-to-many relationship between TvShow and Genre
/// One TV show can have multiple genres
/// One genre can have multiple TV shows
/// </summary>
public class TvShowGenre
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
    /// Foreign key to Genre
    /// </summary>
    public Guid GenreId { get; set; }

    /// <summary>
    /// Navigation property to Genre
    /// </summary>
    public Genre Genre { get; set; } = null!;
}