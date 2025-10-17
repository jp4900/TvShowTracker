namespace TvShowTrackerAPI.Models;

/// <summary>
/// Base entity class that all database models inherit from.
/// Provides common fields: Id, CreatedAt, UpdatedAt, IsDeleted
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp for creation of the entity
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp for last update of the entity -nullable
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag - instead of deleting from DB, we mark as deleted
    /// GDPR Compliance: Allows us to track deletion requests
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}