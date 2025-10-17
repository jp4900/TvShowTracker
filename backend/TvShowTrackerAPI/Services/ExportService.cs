using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using TvShowTrackerAPI.Data;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Export service implementation
/// </summary>
public class ExportService : IExportService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ExportService> _logger;

    public ExportService(AppDbContext context, ILogger<ExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Export user's complete data to CSV
    /// </summary>
    public async Task<byte[]> ExportUserDataToCsvAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.FavoriteTvShows)
                    .ThenInclude(f => f.TvShow)
                        .ThenInclude(t => t.Genres)
                            .ThenInclude(tg => tg.Genre)
                .Include(u => u.Recommendations)
                    .ThenInclude(r => r.TvShow)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("User not found");

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            });

            // Section 1: User Profile
            await streamWriter.WriteLineAsync("=== USER PROFILE ===");
            csv.WriteField("Field");
            csv.WriteField("Value");
            await csv.NextRecordAsync();

            csv.WriteField("User ID");
            csv.WriteField(user.Id);
            await csv.NextRecordAsync();

            csv.WriteField("Email");
            csv.WriteField(user.Email);
            await csv.NextRecordAsync();

            csv.WriteField("Account Created");
            csv.WriteField(user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            await csv.NextRecordAsync();

            csv.WriteField("Last Login");
            csv.WriteField(user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never");
            await csv.NextRecordAsync();

            csv.WriteField("Data Processing Consent");
            csv.WriteField(user.DataProcessingConsent ? "Yes" : "No");
            await csv.NextRecordAsync();

            csv.WriteField("Consent Date");
            csv.WriteField(user.ConsentDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A");
            await csv.NextRecordAsync();

            // Section 2: Favorite TV Shows
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync("=== FAVORITE TV SHOWS ===");

            csv.WriteField("Title");
            csv.WriteField("Rating");
            csv.WriteField("Release Date");
            csv.WriteField("Genres");
            csv.WriteField("Added to Favorites");
            await csv.NextRecordAsync();

            foreach (var favorite in user.FavoriteTvShows.OrderByDescending(f => f.AddedAt))
            {
                csv.WriteField(favorite.TvShow.Title);
                csv.WriteField(favorite.TvShow.Rating);
                csv.WriteField(favorite.TvShow.ReleaseDate?.ToString("yyyy-MM-dd") ?? "Unknown");
                csv.WriteField(string.Join(", ", favorite.TvShow.Genres.Select(g => g.Genre.Name)));
                csv.WriteField(favorite.AddedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                await csv.NextRecordAsync();
            }

            // Section 3: Recommendations
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync("=== RECOMMENDATIONS ===");

            csv.WriteField("Title");
            csv.WriteField("Score");
            csv.WriteField("Reason");
            csv.WriteField("Generated At");
            await csv.NextRecordAsync();

            foreach (var rec in user.Recommendations.OrderByDescending(r => r.Score).Take(20))
            {
                csv.WriteField(rec.TvShow.Title);
                csv.WriteField(rec.Score);
                csv.WriteField(rec.Reason ?? "N/A");
                csv.WriteField(rec.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                await csv.NextRecordAsync();
            }

            await streamWriter.FlushAsync();

            _logger.LogInformation("User data exported to CSV for user {UserId}", userId);

            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user data to CSV for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Export user's complete data to PDF
    /// </summary>
    public async Task<byte[]> ExportUserDataToPdfAsync(Guid userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.FavoriteTvShows)
                    .ThenInclude(f => f.TvShow)
                        .ThenInclude(t => t.Genres)
                            .ThenInclude(tg => tg.Genre)
                .Include(u => u.Recommendations)
                    .ThenInclude(r => r.TvShow)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("User not found");

            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 50, 50);
            var writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20);
            var title = new Paragraph("TV Show Tracker - User Data Export", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Export date
            var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(128, 128, 128));
            var exportDate = new Paragraph($"Exported on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC", dateFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 30
            };
            document.Add(exportDate);

            // Section 1: User Profile
            AddSectionTitle(document, "User Profile");

            var profileTable = new PdfPTable(2) { WidthPercentage = 100, SpacingAfter = 20 };
            profileTable.SetWidths(new float[] { 1, 2 });

            AddTableRow(profileTable, "User ID:", user.Id.ToString());
            AddTableRow(profileTable, "Email:", user.Email);
            AddTableRow(profileTable, "Account Created:", user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            AddTableRow(profileTable, "Last Login:", user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never");
            AddTableRow(profileTable, "Data Consent:", user.DataProcessingConsent ? "Yes" : "No");
            AddTableRow(profileTable, "Consent Date:", user.ConsentDate?.ToString("yyyy-MM-dd") ?? "N/A");

            document.Add(profileTable);

            // Section 2: Favorite TV Shows
            AddSectionTitle(document, $"Favorite TV Shows ({user.FavoriteTvShows.Count})");

            if (user.FavoriteTvShows.Any())
            {
                var favoritesTable = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 20 };
                favoritesTable.SetWidths(new float[] { 3, 1, 2, 2 });

                AddTableHeader(favoritesTable, "Title", "Rating", "Genres", "Added");

                foreach (var favorite in user.FavoriteTvShows.OrderByDescending(f => f.AddedAt))
                {
                    AddTableRow(favoritesTable,
                        favorite.TvShow.Title,
                        favorite.TvShow.Rating.ToString("F1"),
                        string.Join(", ", favorite.TvShow.Genres.Select(g => g.Genre.Name)),
                        favorite.AddedAt.ToString("yyyy-MM-dd"));
                }

                document.Add(favoritesTable);
            }
            else
            {
                document.Add(new Paragraph("No favorites yet.", FontFactory.GetFont(FontFactory.HELVETICA, 12))
                {
                    SpacingAfter = 20
                });
            }

            // Section 3: Recommendations
            var topRecommendations = user.Recommendations.OrderByDescending(r => r.Score).Take(20).ToList();

            AddSectionTitle(document, $"Top Recommendations ({topRecommendations.Count})");

            if (topRecommendations.Any())
            {
                var recsTable = new PdfPTable(3) { WidthPercentage = 100, SpacingAfter = 20 };
                recsTable.SetWidths(new float[] { 3, 1, 3 });

                AddTableHeader(recsTable, "Title", "Score", "Reason");

                foreach (var rec in topRecommendations)
                {
                    AddTableRow(recsTable,
                        rec.TvShow.Title,
                        rec.Score.ToString("F1"),
                        rec.Reason ?? "Recommended for you");
                }

                document.Add(recsTable);
            }
            else
            {
                document.Add(new Paragraph("No recommendations yet. Add some favorites to get personalized recommendations!",
                    FontFactory.GetFont(FontFactory.HELVETICA, 12))
                {
                    SpacingAfter = 20
                });
            }

            // Footer
            document.Add(new Paragraph("\n"));
            var footer = new Paragraph("This document contains your personal data as stored in TV Show Tracker. " +
                                     "You have the right to request corrections or deletion of this data (GDPR).",
                FontFactory.GetFont(FontFactory.HELVETICA, 8, new BaseColor(128, 128, 128)))
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            document.Add(footer);

            document.Close();
            writer.Close();

            _logger.LogInformation("User data exported to PDF for user {UserId}", userId);

            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting user data to PDF for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Export TV shows to CSV
    /// </summary>
    public async Task<byte[]> ExportTvShowsToCsvAsync(List<Guid> tvShowIds)
    {
        try
        {
            var tvShows = await _context.TvShows
                .Include(t => t.Genres)
                    .ThenInclude(tg => tg.Genre)
                .Where(t => tvShowIds.Contains(t.Id))
                .OrderBy(t => t.Title)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            using var csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

            // Write records
            await csv.WriteRecordsAsync(tvShows.Select(t => new
            {
                t.Title,
                Rating = t.Rating,
                ReleaseDate = t.ReleaseDate?.ToString("yyyy-MM-dd") ?? "Unknown",
                Status = t.Status.ToString(),
                Type = t.Type.ToString(),
                Genres = string.Join(", ", t.Genres.Select(g => g.Genre.Name)),
                NumberOfSeasons = t.NumberOfSeasons,
                NumberOfEpisodes = t.NumberOfEpisodes,
                Popularity = t.Popularity
            }));

            await streamWriter.FlushAsync();
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting TV shows to CSV");
            throw;
        }
    }

    /// <summary>
    /// Export TV shows to PDF
    /// </summary>
    public async Task<byte[]> ExportTvShowsToPdfAsync(List<Guid> tvShowIds)
    {
        try
        {
            var tvShows = await _context.TvShows
                .Include(t => t.Genres)
                    .ThenInclude(tg => tg.Genre)
                .Where(t => tvShowIds.Contains(t.Id))
                .OrderBy(t => t.Title)
                .ToListAsync();

            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate(), 30, 30, 30, 30); // Landscape
            var writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            // Title
            var title = new Paragraph("TV Shows Export", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            // Table
            var table = new PdfPTable(6) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 3, 1, 1.5f, 1.5f, 2, 1 });

            AddTableHeader(table, "Title", "Rating", "Release Date", "Status", "Genres", "Seasons");

            foreach (var show in tvShows)
            {
                AddTableRow(table,
                    show.Title,
                    show.Rating.ToString("F1"),
                    show.ReleaseDate?.ToString("yyyy-MM-dd") ?? "Unknown",
                    show.Status.ToString(),
                    string.Join(", ", show.Genres.Select(g => g.Genre.Name)),
                    show.NumberOfSeasons?.ToString() ?? "N/A");
            }

            document.Add(table);

            document.Close();
            writer.Close();

            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting TV shows to PDF");
            throw;
        }
    }

    // Helper methods for PDF generation
    private void AddSectionTitle(Document document, string title)
    {
        var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
        var section = new Paragraph(title, sectionFont)
        {
            SpacingBefore = 10,
            SpacingAfter = 10
        };
        document.Add(section);
    }

    private void AddTableHeader(PdfPTable table, params string[] headers)
    {
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, new BaseColor(255, 255, 255));

        foreach (var header in headers)
        {
            var cell = new PdfPCell(new Phrase(header, headerFont))
            {
                BackgroundColor = new BaseColor(70, 130, 180),
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(cell);
        }
    }

    private void AddTableRow(PdfPTable table, params string[] values)
    {
        var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

        foreach (var value in values)
        {
            var cell = new PdfPCell(new Phrase(value, cellFont))
            {
                Padding = 5
            };
            table.AddCell(cell);
        }
    }
}