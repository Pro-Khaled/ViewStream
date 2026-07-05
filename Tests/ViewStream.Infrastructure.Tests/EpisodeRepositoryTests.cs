using FluentAssertions;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Repositories;

namespace ViewStream.Infrastructure.Tests
{
    public class EpisodeRepositoryTests : IntegrationTestBase
    {
        private readonly EpisodeRepository _repository;

        public EpisodeRepositoryTests(DatabaseFixture fixture) : base(fixture)
        {
            _repository = new EpisodeRepository(DbContext);
        }

        [Fact]
        public async Task AddAsync_ShouldInsertEpisodeIntoDatabase()
        {
            // Arrange: We must create a Show and a Season first due to foreign key constraints.
            var show = new Show
            {
                Title = "Test Show",
                Description = "A show for testing",
                ReleaseYear = 2026
            };
            await DbContext.Set<Show>().AddAsync(show);
            await DbContext.SaveChangesAsync();

            var season = new Season
            {
                ShowId = show.Id,
                SeasonNumber = 1,
                Title = "Season 1"
            };
            await DbContext.Set<Season>().AddAsync(season);
            await DbContext.SaveChangesAsync();

            var episode = new Episode
            {
                SeasonId = season.Id,
                EpisodeNumber = 1,
                Title = "Integration Test Episode",
                VideoUrl = "https://integration.test/video.mp4"
            };

            // Act
            await _repository.AddAsync(episode);
            await DbContext.SaveChangesAsync();

            // Assert
            var result = await _repository.GetByIdAsync(episode.Id);
            result.Should().NotBeNull();
            result!.Title.Should().Be("Integration Test Episode");
            result.EpisodeNumber.Should().Be(1);
            result.SeasonId.Should().Be(season.Id);
        }

        [Fact]
        public async Task AnyAsync_WithExistingEpisode_ShouldReturnTrue()
        {
            // Arrange
            var show = new Show
            {
                Title = "Test Show 2",
                Description = "Another test show",
                ReleaseYear = 2026
            };
            await DbContext.Set<Show>().AddAsync(show);
            await DbContext.SaveChangesAsync();

            var season = new Season
            {
                ShowId = show.Id,
                SeasonNumber = 2,
                Title = "Season 2"
            };
            await DbContext.Set<Season>().AddAsync(season);
            await DbContext.SaveChangesAsync();

            var episode = new Episode
            {
                SeasonId = season.Id,
                EpisodeNumber = 3,
                Title = "Another Episode",
                VideoUrl = "https://integration.test/video2.mp4"
            };
            await _repository.AddAsync(episode);
            await DbContext.SaveChangesAsync();

            // Act
            var exists = await _repository.AnyAsync(e => e.Title == "Another Episode");

            // Assert
            exists.Should().BeTrue();
        }
    }
}
