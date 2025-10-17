namespace TvShowTrackerAPI.DTOs.TvShows;

/// <summary>
/// DTO for TV Show responses
/// </summary>
public class TvShowDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ReleaseDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public string? BackdropUrl { get; set; }
    public decimal Rating { get; set; }
    public int VoteCount { get; set; }
    public int? NumberOfSeasons { get; set; }
    public int? NumberOfEpisodes { get; set; }
    public decimal Popularity { get; set; }

    /// <summary>
    /// List of genres
    /// </summary>
    public List<string> Genres { get; set; } = new();

    /// <summary>
    /// Whether current user has favorited this show
    /// </summary>
    public bool IsFavorite { get; set; }
}

/// <summary>
/// DTO for TV Show list item ()used for lists/tables
/// </summary>
public class TvShowListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public decimal Rating { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public List<string> Genres { get; set; } = new();
    public bool IsFavorite { get; set; }
}