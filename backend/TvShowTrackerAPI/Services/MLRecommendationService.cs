using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using TvShowTrackerAPI.Data;
using TvShowTrackerAPI.Models;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Machine Learning Recommendation Service using ML.NET
/// 
/// 1. Trains on user-show interactions (favorites = implicit ratings)
/// 2. Learns latent features about users and shows
/// 3. Predicts which shows a user would like
/// 4. Combines ML predictions with content-based features
/// </summary>
public class MLRecommendationService : IRecommendationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MLRecommendationService> _logger;
    private readonly MLContext _mlContext;
    private ITransformer? _model;
    private readonly string _modelPath = "recommendation_model.zip";

    public MLRecommendationService(AppDbContext context, ILogger<MLRecommendationService> logger)
    {
        _context = context;
        _logger = logger;
        _mlContext = new MLContext(seed: 0);

        // Try to load existing model
        if (File.Exists(_modelPath))
        {
            _model = _mlContext.Model.Load(_modelPath, out _);
            _logger.LogInformation("✓ ML model loaded from disk");
        }
    }

    /// <summary>
    /// Get ML-powered recommendations
    /// </summary>
    public async Task<List<TvShowRecommendationDto>> GetRecommendationsAsync(Guid userId, int count = 10)
    {
        // Check if user exists
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            return new List<TvShowRecommendationDto>();
        }

        // Get user's favorites to exclude
        var favoriteShowIds = await _context.FavoriteTvShows
            .Where(f => f.UserId == userId)
            .Select(f => f.TvShowId)
            .ToListAsync();

        // If no model trained yet, use content-based fallback
        if (_model == null)
        {
            _logger.LogWarning("ML model not trained yet, using content-based recommendations");
            return await GetContentBasedRecommendationsAsync(userId, count, favoriteShowIds);
        }

        // Get all shows user hasn't favorited
        var allShows = await _context.TvShows
            .Include(t => t.Genres)
                .ThenInclude(tg => tg.Genre)
            .Where(t => !favoriteShowIds.Contains(t.Id))
            .ToListAsync();

        if (!allShows.Any())
        {
            return new List<TvShowRecommendationDto>();
        }

        // Create prediction engine
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<TvShowRating, TvShowRatingPrediction>(_model);

        // Score all candidate shows
        var scoredShows = new List<(TvShow Show, float Score)>();

        foreach (var show in allShows)
        {
            try
            {
                var prediction = predictionEngine.Predict(new TvShowRating
                {
                    UserId = userId.ToString(),
                    TvShowId = show.Id.ToString(),
                    Rating = 0 // not yet predicted
                });

                float mlScore = prediction.Score;
                if (float.IsNaN(mlScore) || float.IsInfinity(mlScore))
                {
                    mlScore = 0f;
                }

                mlScore = Math.Max(0f, Math.Min(mlScore, 10f));

                float combinedScore =
                    (mlScore * 0.6f) +                   // 60% ML prediction
                    ((float)show.Rating * 0.2f) +        // 20% show rating
                    ((float)show.Popularity / 10 * 0.2f); // 20% popularity

                scoredShows.Add((show, combinedScore));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error predicting for show {ShowId}", show.Id);
                float fallbackScore = ((float)show.Rating * 0.5f) + ((float)show.Popularity / 10 * 0.5f);
                scoredShows.Add((show, fallbackScore));
            }
        }

        // Get top recommendations
        var topRecommendations = scoredShows
            .OrderByDescending(s => s.Score)
            .Take(count)
            .ToList();

        // Generate reasons based on user's favorites
        var favoriteGenreIds = await _context.FavoriteTvShows
            .Where(f => f.UserId == userId)
            .SelectMany(f => f.TvShow.Genres.Select(g => g.GenreId))
            .Distinct()
            .ToListAsync();

        return topRecommendations.Select(r =>
        {
            float safeScore = Math.Max(0f, Math.Min(r.Score, 10f));
            decimal decimalScore = 0m;

            try
            {
                decimalScore = Math.Round((decimal)safeScore * 10m, 1);
            }
            catch
            {
                decimalScore = 0m;
            }

            return new TvShowRecommendationDto
            {
                Id = r.Show.Id,
                Title = r.Show.Title,
                PosterUrl = r.Show.PosterUrl,
                Rating = r.Show.Rating,
                ReleaseDate = r.Show.ReleaseDate,
                Genres = r.Show.Genres.Select(g => g.Genre.Name).ToList(),
                IsFavorite = false,
                Score = decimalScore,
                Reason = GenerateReason(r.Show, favoriteGenreIds)
            };
        }).ToList();
    }

    /// <summary>
    /// Train the ML.NET model on user favorites
    /// This should be called periodically (e.g., daily via background job)
    /// </summary>
    public async Task<bool> TrainModelAsync()
    {
        _logger.LogInformation("Starting ML model training...");

        try
        {
            // Get training data (user favorites)
            var trainingData = await _context.FavoriteTvShows
                .Select(f => new TvShowRating
                {
                    UserId = f.UserId.ToString(),
                    TvShowId = f.TvShowId.ToString(),
                    Rating = 1.0f // Favorite = positive rating
                })
                .ToListAsync();

            if (trainingData.Count < 10)
            {
                _logger.LogWarning("Not enough data to train model (need at least 10 favorites)");
                return false;
            }

            // Load data into ML.NET
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Define training pipeline
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "UserIdEncoded",
                MatrixRowIndexColumnName = "TvShowIdEncoded",
                LabelColumnName = "Rating",
                NumberOfIterations = 20,
                ApproximationRank = 100,
                LearningRate = 0.1
            };

            var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey("UserIdEncoded", "UserId")
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("TvShowIdEncoded", "TvShowId"))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(options));

            // Train model
            _logger.LogInformation("Training on {Count} favorites...", trainingData.Count);
            _model = pipeline.Fit(dataView);

            // Save model to disk
            _mlContext.Model.Save(_model, dataView.Schema, _modelPath);

            _logger.LogInformation("ML model trained and saved successfully!");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error training ML model");
            return false;
        }
    }

    /// <summary>
    /// Generate recommendations for all users (background job)
    /// </summary>
    public async Task GenerateRecommendationsForAllUsersAsync()
    {
        _logger.LogInformation("Starting recommendation generation for all users...");

        var users = await _context.Users
            .Where(u => !u.IsDeleted)
            .Select(u => u.Id)
            .ToListAsync();

        foreach (var userId in users)
        {
            try
            {
                var recommendations = await GetRecommendationsAsync(userId, 10);

                // Delete old recommendations
                var oldRecommendations = await _context.UserRecommendations
                    .Where(r => r.UserId == userId)
                    .ToListAsync();
                _context.UserRecommendations.RemoveRange(oldRecommendations);

                // Save new recommendations
                foreach (var rec in recommendations)
                {
                    var userRec = new UserRecommendation
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        TvShowId = rec.Id,
                        Score = rec.Score,
                        Reason = rec.Reason,
                        EmailSent = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.UserRecommendations.Add(userRec);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating recommendations for user {UserId}", userId);
            }
        }

        _logger.LogInformation("Finished recommendation generation for {UserCount} users", users.Count);
    }

    /// <summary>
    /// Content-based fallback when ML model isn't available
    /// </summary>
    private async Task<List<TvShowRecommendationDto>> GetContentBasedRecommendationsAsync(
        Guid userId, int count, List<Guid> excludeIds)
    {
        var favoriteGenres = await _context.FavoriteTvShows
            .Where(f => f.UserId == userId)
            .SelectMany(f => f.TvShow.Genres.Select(g => g.GenreId))
            .Distinct()
            .ToListAsync();

        if (!favoriteGenres.Any())
        {
            // Return popular shows
            var popular = await _context.TvShows
                .Include(t => t.Genres).ThenInclude(tg => tg.Genre)
                .Where(t => !excludeIds.Contains(t.Id))
                .OrderByDescending(t => t.Popularity)
                .Take(count)
                .ToListAsync();

            return popular.Select(t => new TvShowRecommendationDto
            {
                Id = t.Id,
                Title = t.Title,
                PosterUrl = t.PosterUrl,
                Rating = t.Rating,
                ReleaseDate = t.ReleaseDate,
                Genres = t.Genres.Select(g => g.Genre.Name).ToList(),
                IsFavorite = false,
                Score = t.Popularity,
                Reason = "Popular show you might enjoy"
            }).ToList();
        }

        // Content-based: similar genres
        var similar = await _context.TvShows
            .Include(t => t.Genres).ThenInclude(tg => tg.Genre)
            .Where(t => !excludeIds.Contains(t.Id) && t.Genres.Any(g => favoriteGenres.Contains(g.GenreId)))
            .OrderByDescending(t => t.Rating)
            .Take(count)
            .ToListAsync();

        return similar.Select(t => new TvShowRecommendationDto
        {
            Id = t.Id,
            Title = t.Title,
            PosterUrl = t.PosterUrl,
            Rating = t.Rating,
            ReleaseDate = t.ReleaseDate,
            Genres = t.Genres.Select(g => g.Genre.Name).ToList(),
            IsFavorite = false,
            Score = t.Rating * 10,
            Reason = $"Similar to your favorite {string.Join(", ", t.Genres.Take(2).Select(g => g.Genre.Name))} shows"
        }).ToList();
    }

    /// <summary>
    /// Generate human-readable reason for recommendation
    /// </summary>
    private string GenerateReason(TvShow show, List<Guid> userFavoriteGenres)
    {
        var matchingGenres = show.Genres
            .Where(g => userFavoriteGenres.Contains(g.GenreId))
            .Select(g => g.Genre.Name)
            .Take(2)
            .ToList();

        if (matchingGenres.Any())
        {
            return $"Based on your love for {string.Join(" and ", matchingGenres)}";
        }

        return $"Highly rated {show.Type} show";
    }
}

/// <summary>
/// Training data class for ML.NET
/// </summary>
public class TvShowRating
{
    public string UserId { get; set; } = string.Empty;
    public string TvShowId { get; set; } = string.Empty;
    public float Rating { get; set; }
}

/// <summary>
/// Prediction output class for ML.NET
/// </summary>
public class TvShowRatingPrediction
{
    public float Score { get; set; }
}