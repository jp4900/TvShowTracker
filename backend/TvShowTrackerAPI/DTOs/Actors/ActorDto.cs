namespace TvShowTrackerAPI.DTOs.Actors;

/// <summary>
/// DTO for Actor responses
/// </summary>
public class ActorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? ProfilePath { get; set; }
    public decimal? Popularity { get; set; }

    /// <summary>
    /// Character name (when fetched from a show context)
    /// </summary>
    public string? CharacterName { get; set; }

    /// <summary>
    /// List of TV shows this actor appeared in
    /// </summary>
    public List<ActorTvShowDto>? TvShows { get; set; }
}

/// <summary>
/// DTO for Actor list item
/// </summary>
public class ActorListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ProfilePath { get; set; }
    public decimal? Popularity { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public int TvShowCount { get; set; }
}

/// <summary>
/// DTO for TV shows in actor's profile
/// </summary>
public class ActorTvShowDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public string? CharacterName { get; set; }
    public decimal Rating { get; set; }
}