namespace TvShowTrackerAPI.Models;

/// <summary>
/// Episode entity
/// </summary>
public class Episode : BaseEntity
{
    /// <summary>
    /// Foreign key to TV Show
    /// </summary>
    public Guid TvShowId { get; set; }

    /// <summary>
    /// Navigation property to TV Show
    /// </summary>
    public TvShow TvShow { get; set; } = null!;

    /// <summary>
    /// Episode title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Episode description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Season number
    /// </summary>
    public int SeasonNumber { get; set; }

    /// <summary>
    /// Episode number within the season
    /// </summary>
    public int EpisodeNumber { get; set; }

    /// <summary>
    /// Release date of the episode
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// Length of the episode in minutes
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// URL to episode still/screenshot
    /// </summary>
    public string? StillPath { get; set; }

    /// <summary>
    /// Episode rating (0-10 scale)
    /// </summary>
    public decimal? Rating { get; set; }

    /// <summary>
    /// Number of votes for this episode
    /// </summary>
    public int? VoteCount { get; set; }
}