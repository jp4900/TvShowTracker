using TvShowTrackerAPI.Models;
using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;


namespace TvShowTrackerAPI.Data;

/// <summary>
/// Database seeder - populates database with sample data
/// </summary>
public static class DbSeeder
{

    private const string TMDB_IMAGE_BASE = "https://image.tmdb.org/t/p/w500";

    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if data already exists
        if (context.TvShows.Any())
        {
            Console.WriteLine("✓ Database already seeded");
            return;
        }

        Console.WriteLine("🌱 Seeding database...");

        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Drama", Description = "Dramatic storytelling" },
            new() { Id = Guid.NewGuid(), Name = "Crime", Description = "Crime and thriller" },
            new() { Id = Guid.NewGuid(), Name = "Comedy", Description = "Comedic content" },
            new() { Id = Guid.NewGuid(), Name = "Sci-Fi", Description = "Science fiction" },
            new() { Id = Guid.NewGuid(), Name = "Fantasy", Description = "Fantasy worlds" },
            new() { Id = Guid.NewGuid(), Name = "Action", Description = "Action-packed" },
            new() { Id = Guid.NewGuid(), Name = "Thriller", Description = "Suspenseful" },
            new() { Id = Guid.NewGuid(), Name = "Mystery", Description = "Mystery solving" }
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();

        var actors = new List<Actor>
        {
            new() { Id = Guid.NewGuid(), Name = "Bryan Cranston", DateOfBirth = Utc(1956, 3, 7), PlaceOfBirth = "California, USA", Popularity = 95.5m, ProfilePath = "https://image.tmdb.org/t/p/w500/7Jahy5LZX2Fo8fGJltMleIO1078.jpg" },
new() { Id = Guid.NewGuid(), Name = "Aaron Paul", DateOfBirth = Utc(1979, 8, 27), PlaceOfBirth = "Idaho, USA", Popularity = 88.3m, ProfilePath = "https://image.tmdb.org/t/p/w500/oTceEUb6A9Bg6DeUTJBTETUOEFu.jpg" },
new() { Id = Guid.NewGuid(), Name = "Emilia Clarke", DateOfBirth = Utc(1986, 10, 23), PlaceOfBirth = "London, UK", Popularity = 92.7m, ProfilePath = "https://image.tmdb.org/t/p/w500/j7d083zIMhwnKro3tQqDz2Fq1UD.jpg" },
new() { Id = Guid.NewGuid(), Name = "Kit Harington", DateOfBirth = Utc(1986, 12, 26), PlaceOfBirth = "London, UK", Popularity = 89.1m, ProfilePath = "https://image.tmdb.org/t/p/w500/4MnRgrwuiJvHikgunRplrabkV8o.jpg" },
new() { Id = Guid.NewGuid(), Name = "Pedro Pascal", DateOfBirth = Utc(1975, 4, 2), PlaceOfBirth = "Chile", Popularity = 96.8m, ProfilePath = "https://image.tmdb.org/t/p/w500/9VYK7oxcqhjd5LAH6ZFJ3XzOlID.jpg" },
new() { Id = Guid.NewGuid(), Name = "Bella Ramsey", DateOfBirth = Utc(2003, 9, 30), PlaceOfBirth = "Nottingham, UK", Popularity = 85.2m, ProfilePath = "https://image.tmdb.org/t/p/w500/sxAvB53nMXo9PuHYiVsYtNwlMUN.jpg" },
new() { Id = Guid.NewGuid(), Name = "Bob Odenkirk", DateOfBirth = Utc(1962, 10, 22), PlaceOfBirth = "Illinois, USA", Popularity = 87.9m, ProfilePath = "https://image.tmdb.org/t/p/w500/2eWvD2fHijF3og0agKlFL8ScKwr.jpg" },
new() { Id = Guid.NewGuid(), Name = "Rhea Seehorn", DateOfBirth = Utc(1972, 5, 12), PlaceOfBirth = "Virginia, USA", Popularity = 82.4m, ProfilePath = "https://image.tmdb.org/t/p/w500/7htN1iFEgJXOY0jDR5eduBad3o5.jpg" }
        };

        await context.Actors.AddRangeAsync(actors);
        await context.SaveChangesAsync();

        var tvShows = new List<TvShow>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Breaking Bad",
                Description = "A high school chemistry teacher diagnosed with inoperable lung cancer turns to manufacturing and selling methamphetamine.",
                ReleaseDate = Utc(2008, 1, 20),
                EndDate = Utc(2013, 9, 29),
                Status = TvShowStatus.Ended,
                Type = TvShowType.Scripted,
                Rating = 9.5m,
                VoteCount = 1500000,
                NumberOfSeasons = 5,
                NumberOfEpisodes = 62,
                Popularity = 98.5m,
                PosterUrl = $"{TMDB_IMAGE_BASE}/ggFHVNu6YYI5L9pCfOacjizRGt.jpg",
                BackdropUrl = $"{TMDB_IMAGE_BASE}/tsRy63Mu5cu8etL1X7ZLyf7UP1M.jpg"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Game of Thrones",
                Description = "Nine noble families fight for control over the lands of Westeros, while an ancient enemy returns.",
                ReleaseDate = Utc(2011, 4, 17),
                EndDate = Utc(2019, 5, 19),
                Status = TvShowStatus.Ended,
                Type = TvShowType.Scripted,
                Rating = 9.2m,
                VoteCount = 2000000,
                NumberOfSeasons = 8,
                NumberOfEpisodes = 73,
                Popularity = 97.8m,
                PosterUrl = $"{TMDB_IMAGE_BASE}/1XS1oqL89opfnbLl8WnZY1O1uJx.jpg",
                BackdropUrl = $"{TMDB_IMAGE_BASE}/2OMB0ynKlyIenMJWI2Dy9IWT4c.jpg"
            
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "The Last of Us",
                Description = "After a global pandemic destroys civilization, a hardened survivor takes charge of a 14-year-old girl.",
                ReleaseDate = Utc(2023, 1, 15),
                EndDate = null,
                Status = TvShowStatus.Returning,
                Type = TvShowType.Scripted,
                Rating = 8.8m,
                VoteCount = 450000,
                NumberOfSeasons = 1,
                NumberOfEpisodes = 9,
                Popularity = 95.2m,
                PosterUrl = $"{TMDB_IMAGE_BASE}/uKvVjHNqB5VmOrdxqAt2F7J78ED.jpg",
                BackdropUrl = $"{TMDB_IMAGE_BASE}/uDgy6hyPd82kOHh6I95FLtLnj6p.jpg"
            
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Better Call Saul",
                Description = "The trials and tribulations of criminal lawyer Jimmy McGill in the years leading up to his fateful run-in with Walter White.",
                ReleaseDate = Utc(2015, 2, 8),
                EndDate = Utc(2022, 8, 15),
                Status = TvShowStatus.Ended,
                Type = TvShowType.Scripted,
                Rating = 9.0m,
                VoteCount = 500000,
                NumberOfSeasons = 6,
                NumberOfEpisodes = 63,
                Popularity = 92.1m,
                PosterUrl = $"{TMDB_IMAGE_BASE}/fC2HDm5t0kHl7mTm7jxMR31b7by.jpg",
                BackdropUrl = $"{TMDB_IMAGE_BASE}/9faGSFi5jam6pDWGNd0p8JcJgXQ.jpg"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Stranger Things",
                Description = "When a young boy disappears, his mother, a police chief and his friends must confront terrifying supernatural forces.",
                ReleaseDate = Utc(2016, 7, 15),
                EndDate = null,
                Status = TvShowStatus.Returning,
                Type = TvShowType.Scripted,
                Rating = 8.7m,
                VoteCount = 900000,
                NumberOfSeasons = 4,
                NumberOfEpisodes = 42,
                Popularity = 96.3m,
                PosterUrl = $"{TMDB_IMAGE_BASE}/49WJfeN0moxb9IPfGn8AIqMGskD.jpg",
                BackdropUrl = $"{TMDB_IMAGE_BASE}/56v2KjBlU4XaOv9rVYEQypROD7P.jpg"
            }
        };

        await context.TvShows.AddRangeAsync(tvShows);
        await context.SaveChangesAsync();


        var tvShowGenres = new List<TvShowGenre>
        {
            new() { TvShowId = tvShows[0].Id, GenreId = genres[0].Id },
            new() { TvShowId = tvShows[0].Id, GenreId = genres[1].Id },
            new() { TvShowId = tvShows[0].Id, GenreId = genres[6].Id },
            
            new() { TvShowId = tvShows[1].Id, GenreId = genres[0].Id },
            new() { TvShowId = tvShows[1].Id, GenreId = genres[4].Id },
            new() { TvShowId = tvShows[1].Id, GenreId = genres[5].Id },
            
            new() { TvShowId = tvShows[2].Id, GenreId = genres[0].Id },
            new() { TvShowId = tvShows[2].Id, GenreId = genres[3].Id },
            new() { TvShowId = tvShows[2].Id, GenreId = genres[5].Id },
            
            new() { TvShowId = tvShows[3].Id, GenreId = genres[0].Id },
            new() { TvShowId = tvShows[3].Id, GenreId = genres[1].Id },
            
            new() { TvShowId = tvShows[4].Id, GenreId = genres[3].Id },
            new() { TvShowId = tvShows[4].Id, GenreId = genres[4].Id },
            new() { TvShowId = tvShows[4].Id, GenreId = genres[7].Id }
        };

        await context.TvShowGenres.AddRangeAsync(tvShowGenres);
        await context.SaveChangesAsync();

        var tvShowActors = new List<TvShowActor>
        {
            new() { TvShowId = tvShows[0].Id, ActorId = actors[0].Id, CharacterName = "Walter White", Order = 0 },
            new() { TvShowId = tvShows[0].Id, ActorId = actors[1].Id, CharacterName = "Jesse Pinkman", Order = 1 },
            
            new() { TvShowId = tvShows[1].Id, ActorId = actors[2].Id, CharacterName = "Daenerys Targaryen", Order = 0 },
            new() { TvShowId = tvShows[1].Id, ActorId = actors[3].Id, CharacterName = "Jon Snow", Order = 1 },
            
            new() { TvShowId = tvShows[2].Id, ActorId = actors[4].Id, CharacterName = "Joel Miller", Order = 0 },
            new() { TvShowId = tvShows[2].Id, ActorId = actors[5].Id, CharacterName = "Ellie Williams", Order = 1 },
            
            new() { TvShowId = tvShows[3].Id, ActorId = actors[6].Id, CharacterName = "Jimmy McGill", Order = 0 },
            new() { TvShowId = tvShows[3].Id, ActorId = actors[7].Id, CharacterName = "Kim Wexler", Order = 1 }
        };

        await context.TvShowActors.AddRangeAsync(tvShowActors);
        await context.SaveChangesAsync();

        //sample episodes for Breaking Bad
        var episodes = new List<Episode>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TvShowId = tvShows[0].Id,
                Title = "Pilot",
                Description = "When an unassuming high school chemistry teacher discovers he has cancer, he decides to team up with a former student to secure his family's future.",
                SeasonNumber = 1,
                EpisodeNumber = 1,
                ReleaseDate = Utc(2008, 1, 20),
                Length = 58,
                Rating = 8.9m
            },
            new()
            {
                Id = Guid.NewGuid(),
                TvShowId = tvShows[0].Id,
                Title = "Cat's in the Bag...",
                Description = "Walt and Jesse attempt to tie up loose ends. The desperate situation gets more complicated.",
                SeasonNumber = 1,
                EpisodeNumber = 2,
                ReleaseDate = Utc(2008, 1, 27),
                Length = 48,
                Rating = 8.7m
            },
            new()
            {
                Id = Guid.NewGuid(),
                TvShowId = tvShows[0].Id,
                Title = "...And the Bag's in the River",
                Description = "Walter fights with Jesse over his drug use, causing him to leave Walter alone.",
                SeasonNumber = 2,
                EpisodeNumber = 3,
                ReleaseDate = Utc(2008, 2, 10),
                Length = 48,
                Rating = 8.8m
            }
        };

        var randomTvShows = new List<TvShow>();

        for (int i = 1; i <= 20; i++)
        {
            randomTvShows.Add(new TvShow
            {
                Id = Guid.NewGuid(),
                Title = $"Test Show {i}",
                Description = $"Description for Test Show {i}.",
                ReleaseDate = Utc(2010 + i % 10, i % 12 + 1, i % 28 + 1),
                EndDate = null,
                Status = TvShowStatus.Returning,
                Type = TvShowType.Scripted,
                Rating = 7 + (i % 3) * 0.5m,
                VoteCount = 1000 + i * 50,
                NumberOfSeasons = i % 5 + 1,
                NumberOfEpisodes = (i % 10 + 1) * 10,
                Popularity = 50 + i 
            });
        }

        await context.TvShows.AddRangeAsync(randomTvShows);
        await context.SaveChangesAsync();

        // Add genres randomly to shows
        var random = new Random();
        var tvShowGenresExtra = new List<TvShowGenre>();

        foreach (var show in randomTvShows)
        {
            var genre = genres[random.Next(genres.Count)];
            tvShowGenresExtra.Add(new TvShowGenre { TvShowId = show.Id, GenreId = genre.Id });
        }

        await context.TvShowGenres.AddRangeAsync(tvShowGenresExtra);
        await context.SaveChangesAsync();

        // Add a test user
        var testUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "testuser@example.com",
            PasswordHash = BC.HashPassword("Test1234")
        };

        await context.Users.AddAsync(testUser);
        await context.SaveChangesAsync();

        testUser = await context.Users.FirstAsync(u => u.Email == "testuser@example.com");

        

        // Add episodes last
        await context.Episodes.AddRangeAsync(episodes);
        await context.SaveChangesAsync();

        Console.WriteLine("✓ Database seeded successfully!");
        Console.WriteLine($"  - {genres.Count} genres");
        Console.WriteLine($"  - {actors.Count} actors");
        Console.WriteLine($"  - {tvShows.Count + randomTvShows.Count} TV shows total");
        Console.WriteLine($"  - {episodes.Count} episodes");
        Console.WriteLine($"  - 1 test user ({testUser.Email}, password: Test1234)");
    }
    private static DateTime Utc(int year, int month, int day)
        => DateTime.SpecifyKind(new DateTime(year, month, day), DateTimeKind.Utc);
}

