namespace TvShowTrackerAPI.DTOs.TvShows;

/// <summary>
/// DTO for Episode responses
/// </summary>
public class EpisodeDto
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }

	/// <summary>
	/// Season number (sortable)
	/// </summary>
	public int SeasonNumber { get; set; }

	/// <summary>
	/// Episode number within season (sortable)
	/// </summary>
	public int EpisodeNumber { get; set; }

	/// <summary>
	/// Release date 
	/// </summary>
	public DateTime? ReleaseDate { get; set; }

	/// <summary>
	/// Episode length in minutes
	/// </summary>
	public int? Length { get; set; }

	public string? StillPath { get; set; }
	public decimal? Rating { get; set; }
	public int? VoteCount { get; set; }
}