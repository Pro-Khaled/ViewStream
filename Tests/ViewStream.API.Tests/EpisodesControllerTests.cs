using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Infrastructure.Persistence;
using Xunit;

namespace ViewStream.API.Tests
{
    public class EpisodesControllerTests : ApiTestBase
    {
        public EpisodesControllerTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        private async Task<(long ShowId, long SeasonId, long EpisodeId)> SeedEpisodeAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViewStreamDbContext>();
            optionsBuilder.UseSqlServer(Factory.SqlContainer.GetConnectionString());
            using var context = new ViewStreamDbContext(optionsBuilder.Options);

            var show = new Show
            {
                Title = "E2E Test Show",
                Description = "A show for E2E tests",
                ReleaseYear = 2026
            };
            await context.Set<Show>().AddAsync(show);
            await context.SaveChangesAsync();

            var season = new Season
            {
                ShowId = show.Id,
                SeasonNumber = 1,
                Title = "E2E Season 1"
            };
            await context.Set<Season>().AddAsync(season);
            await context.SaveChangesAsync();

            var episode = new Episode
            {
                SeasonId = season.Id,
                EpisodeNumber = 1,
                Title = "E2E Episode 1",
                VideoUrl = "https://e2e.test/episode1.mp4"
            };
            await context.Set<Episode>().AddAsync(episode);
            await context.SaveChangesAsync();

            return (show.Id, season.Id, episode.Id);
        }

        [Fact]
        public async Task GetEpisode_WithNonExistentId_ShouldReturnNotFound()
        {
            // Act
            var response = await Client.GetAsync("api/v1/Episodes/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetEpisode_WithExistentId_ShouldReturnEpisodeDetails()
        {
            // Arrange
            var ids = await SeedEpisodeAsync();

            // Act
            var response = await Client.GetAsync($"api/v1/Episodes/{ids.EpisodeId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<EpisodeDto>();
            result.Should().NotBeNull();
            result!.Id.Should().Be(ids.EpisodeId);
            result.Title.Should().Be("E2E Episode 1");
        }

        [Fact]
        public async Task GetEpisodeStream_WithJwt_ShouldReturnStreamUrl()
        {
            // Arrange
            var ids = await SeedEpisodeAsync();
            await AuthenticateAsync();

            // Act
            var response = await Client.GetAsync($"api/v1/Episodes/{ids.EpisodeId}/stream");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<EpisodeStreamUrlDto>();
            result.Should().NotBeNull();
            result!.VideoUrl.Should().Be("https://e2e.test/episode1.mp4");
        }
    }
}
