namespace TvShowTrackerAPI.Models;

/// <summary>
/// Join table for User's favorite TV shows
/// </summary>
public class FavoriteTvShow
{
	/// <summary>
	/// Foreign key to User
	/// </summary>
	public Guid UserId { get; set; }

	/// <summary>
	/// Navigation property to User
	/// </summary>
	public User User { get; set; } = null!;

	/// <summary>
	/// Foreign key to TvShow
	/// </summary>
	public Guid TvShowId { get; set; }

	/// <summary>
	/// Navigation property to TvShow
	/// </summary>
	public TvShow TvShow { get; set; } = null!;

	/// <summary>
	/// When the user added this to favorites
	/// </summary>
	public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}