namespace TvShowTrackerAPI.Services;

/// <summary>
/// Interface for export service
/// Requirement #16: Export information to CSV/PDF
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Export user's complete data to CSV
    /// Includes: profile, favorites, recommendations
    /// </summary>
    Task<byte[]> ExportUserDataToCsvAsync(Guid userId);

    /// <summary>
    /// Export user's complete data to PDF
    /// Includes: profile, favorites, recommendations
    /// </summary>
    Task<byte[]> ExportUserDataToPdfAsync(Guid userId);

    /// <summary>
    /// Export TV shows list to CSV
    /// </summary>
    Task<byte[]> ExportTvShowsToCsvAsync(List<Guid> tvShowIds);

    /// <summary>
    /// Export TV shows list to PDF
    /// </summary>
    Task<byte[]> ExportTvShowsToPdfAsync(List<Guid> tvShowIds);
}