namespace TvShowTrackerAPI.Models;

/// <summary>
/// TV Show entity
/// Requirements covered:
/// Returns all TV shows available
/// Returns TV shows by genre, type
/// Sortable by available fields
/// </summary>
public class TvShow : BaseEntity
{
    /// <summary>
    /// TV Show title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of the show
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Release date
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// End date if show has concluded
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Current status of the show (Returning, Ended, Canceled, etc.)
    /// </summary>
    public TvShowStatus Status { get; set; }

    /// <summary>
    /// Type of show (Scripted, Documentary, Reality, Talk Show, etc.)
    /// </summary>
    public TvShowType Type { get; set; }

    /// <summary>
    /// URL to poster image
    /// </summary>
    public string? PosterUrl { get; set; }

    /// <summary>
    /// URL to backdrop/banner image
    /// </summary>
    public string? BackdropUrl { get; set; }

    /// <summary>
    /// Average rating (0-10 scale)
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Number of votes/ratings
    /// </summary>
    public int VoteCount { get; set; }

    /// <summary>
    /// Original language of the show
    /// </summary>
    public string? OriginalLanguage { get; set; }

    /// <summary>
    /// Total number of seasons
    /// </summary>
    public int? NumberOfSeasons { get; set; }

    /// <summary>
    /// Total number of episodes
    /// </summary>
    public int? NumberOfEpisodes { get; set; }

    /// <summary>
    /// Popularity score (for sorting/recommendations)
    /// </summary>
    public decimal Popularity { get; set; }

    // ===========================================
    // NAVIGATION PROPERTIES (Relationships)
    // ===========================================

    /// <summary>
    /// Genres of this TV show (many-to-many)
    /// </summary>
    public ICollection<TvShowGenre> Genres { get; set; } = new List<TvShowGenre>();

    /// <summary>
    /// Episodes of this TV show
    /// </summary>
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();

    /// <summary>
    /// Actors in this TV show (many-to-many)
    /// </summary>
    public ICollection<TvShowActor> Actors { get; set; } = new List<TvShowActor>();

    /// <summary>
    /// Users who favorited this show
    /// </summary>
    public ICollection<FavoriteTvShow> FavoritedBy { get; set; } = new List<FavoriteTvShow>();
}

/// <summary>
/// Status of a TV show
/// </summary>
public enum TvShowStatus
{
    Planned = 1,
    InProduction = 2,
    Returning = 3,
    Ended = 4,
    Canceled = 5
}

/// <summary>
/// Type of TV show
/// </summary>
public enum TvShowType
{
    Scripted = 0,
    Documentary = 1,
    Reality = 2,
    TalkShow = 3,
    GameShow = 4,
    NewsShow = 5,
    Miniseries = 6,
    Other = 99
}