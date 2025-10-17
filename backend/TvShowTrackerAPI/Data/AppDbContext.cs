using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Models;

namespace TvShowTrackerAPI.Data;

/// <summary>
/// Database context for the application
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ===============================================
    // DbSets - Each represents a table in database
    // ===============================================

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<TvShow> TvShows { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<TvShowGenre> TvShowGenres { get; set; }
    public DbSet<TvShowActor> TvShowActors { get; set; }
    public DbSet<FavoriteTvShow> FavoriteTvShows { get; set; }
    public DbSet<UserRecommendation> UserRecommendations { get; set; }

    /// <summary>
    /// Configure entity relationships and constraints
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // USER CONFIGURATION
        // ==========================================

        modelBuilder.Entity<User>(entity =>
        {
            // Email must be unique and indexed for fast lookups
            entity.HasIndex(e => e.Email).IsUnique();

            // Prevent accidental deletion of users who have data
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete tokens when user is deleted

            entity.HasMany(u => u.FavoriteTvShows)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Recommendations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Global query filter - exclude soft-deleted users
            entity.HasQueryFilter(u => !u.IsDeleted);
        });

        // ==========================================
        // REFRESH TOKEN CONFIGURATION
        // ==========================================

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            // Index on token for fast lookups during authentication
            entity.HasIndex(e => e.Token);

            // Index on expiration date for pruning expired tokens
            entity.HasIndex(e => e.ExpiresAt);

            // Index on user + active tokens (for revoking all user tokens)
            entity.HasIndex(e => new { e.UserId, e.IsRevoked, e.ExpiresAt });
        });

        // ==========================================
        // TV SHOW CONFIGURATION
        // ==========================================

        modelBuilder.Entity<TvShow>(entity =>
        {
            // Indexes for filtering and sorting
            entity.HasIndex(e => e.Title); // Search by title
            entity.HasIndex(e => e.Rating); // Sort by rating
            entity.HasIndex(e => e.ReleaseDate); // Sort by release date
            entity.HasIndex(e => e.Popularity); // Sort by popularity
            entity.HasIndex(e => e.Status); // Filter by status
            entity.HasIndex(e => e.Type); // Filter by type

            // Relationships
            entity.HasMany(t => t.Episodes)
                .WithOne(e => e.TvShow)
                .HasForeignKey(e => e.TvShowId)
                .OnDelete(DeleteBehavior.Cascade); // Delete episodes when show is deleted

            // Global query filter - exclude soft-deleted shows
            entity.HasQueryFilter(t => !t.IsDeleted);
        });

        // ==========================================
        // EPISODE CONFIGURATION
        // ==========================================

        modelBuilder.Entity<Episode>(entity =>
        {
            // Index for finding episodes by season/episode number
            entity.HasIndex(e => new { e.TvShowId, e.SeasonNumber, e.EpisodeNumber });

            // Index on release date for sorting
            entity.HasIndex(e => e.ReleaseDate);

            // Global query filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // ==========================================
        // ACTOR CONFIGURATION
        // ==========================================

        modelBuilder.Entity<Actor>(entity =>
        {
            // Index on name for search
            entity.HasIndex(e => e.Name);

            // Index on popularity for sorting
            entity.HasIndex(e => e.Popularity);

            // Global query filter
            entity.HasQueryFilter(a => !a.IsDeleted);
        });

        // ==========================================
        // GENRE CONFIGURATION
        // ==========================================

        modelBuilder.Entity<Genre>(entity =>
        {
            // Genre name must be unique
            entity.HasIndex(e => e.Name).IsUnique();

            // Global query filter
            entity.HasQueryFilter(g => !g.IsDeleted);
        });

        // ==========================================
        // TV SHOW - GENRE (Many-to-Many)
        // ==========================================

        modelBuilder.Entity<TvShowGenre>(entity =>
        {
            // Composite primary key
            entity.HasKey(tg => new { tg.TvShowId, tg.GenreId });

            // Configure relationships
            entity.HasOne(tg => tg.TvShow)
                .WithMany(t => t.Genres)
                .HasForeignKey(tg => tg.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(tg => tg.Genre)
                .WithMany(g => g.TvShows)
                .HasForeignKey(tg => tg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for filtering shows by genre
            entity.HasIndex(tg => tg.GenreId);
        });

        // ==========================================
        // TV SHOW - ACTOR (Many-to-Many)
        // ==========================================

        modelBuilder.Entity<TvShowActor>(entity =>
        {
            // Composite primary key
            entity.HasKey(ta => new { ta.TvShowId, ta.ActorId });

            // Configure relationships
            entity.HasOne(ta => ta.TvShow)
                .WithMany(t => t.Actors)
                .HasForeignKey(ta => ta.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ta => ta.Actor)
                .WithMany(a => a.TvShows)
                .HasForeignKey(ta => ta.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index on Order for getting "featured" actors (lowest order = main role)
            entity.HasIndex(ta => new { ta.TvShowId, ta.Order });
        });

        // ==========================================
        // FAVORITE TV SHOW (Many-to-Many)
        // ==========================================

        modelBuilder.Entity<FavoriteTvShow>(entity =>
        {
            // Composite primary key
            entity.HasKey(f => new { f.UserId, f.TvShowId });

            // Configure relationships
            entity.HasOne(f => f.User)
                .WithMany(u => u.FavoriteTvShows)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.TvShow)
                .WithMany(t => t.FavoritedBy)
                .HasForeignKey(f => f.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for getting user's favorites ordered by date added
            entity.HasIndex(f => new { f.UserId, f.AddedAt });
        });

        // ==========================================
        // USER RECOMMENDATION
        // ==========================================

        modelBuilder.Entity<UserRecommendation>(entity =>
        {
            // Composite index for user recommendations ordered by score
            entity.HasIndex(r => new { r.UserId, r.Score });

            // Index for finding unsent email recommendations
            entity.HasIndex(r => new { r.EmailSent, r.CreatedAt });

            // Configure relationships
            entity.HasOne(r => r.User)
                .WithMany(u => u.Recommendations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.TvShow)
                .WithMany()
                .HasForeignKey(r => r.TvShowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Global query filter
            entity.HasQueryFilter(r => !r.IsDeleted);
        });
    }

    /// <summary>
    /// Automatically set UpdatedAt timestamp when entity is modified
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}