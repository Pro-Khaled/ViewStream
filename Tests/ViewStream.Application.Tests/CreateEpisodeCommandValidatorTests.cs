using FluentValidation.TestHelper;
using ViewStream.Application.Commands.Episode.CreateEpisode;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Tests
{
    public class CreateEpisodeCommandValidatorTests
    {
        private readonly CreateEpisodeCommandValidator _validator;

        public CreateEpisodeCommandValidatorTests()
        {
            _validator = new CreateEpisodeCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var dto = new CreateEpisodeDto
            {
                Title = "",
                SeasonId = 1,
                EpisodeNumber = 1
            };
            var command = new CreateEpisodeCommand(dto, 100);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Title);
        }

        [Fact]
        public void Should_Have_Error_When_Title_Exceeds_500_Characters()
        {
            var dto = new CreateEpisodeDto
            {
                Title = new string('a', 501),
                SeasonId = 1,
                EpisodeNumber = 1
            };
            var command = new CreateEpisodeCommand(dto, 100);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Title);
        }

        [Fact]
        public void Should_Have_Error_When_SeasonId_Is_Zero_Or_Negative()
        {
            var dto = new CreateEpisodeDto
            {
                Title = "Test Title",
                SeasonId = 0,
                EpisodeNumber = 1
            };
            var command = new CreateEpisodeCommand(dto, 100);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.SeasonId);
        }

        [Fact]
        public void Should_Have_Error_When_EpisodeNumber_Is_Zero_Or_Negative()
        {
            var dto = new CreateEpisodeDto
            {
                Title = "Test Title",
                SeasonId = 1,
                EpisodeNumber = 0
            };
            var command = new CreateEpisodeCommand(dto, 100);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.EpisodeNumber);
        }

        [Fact]
        public void Should_Have_Error_When_RuntimeSeconds_Is_Negative()
        {
            var dto = new CreateEpisodeDto
            {
                Title = "Test Title",
                SeasonId = 1,
                EpisodeNumber = 1,
                RuntimeSeconds = -5
            };
            var command = new CreateEpisodeCommand(dto, 100);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.RuntimeSeconds);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var dto = new CreateEpisodeDto
            {
                Title = "Valid Title",
                SeasonId = 1,
                EpisodeNumber = 2,
                RuntimeSeconds = 1200
            };
            var command = new CreateEpisodeCommand(dto, 100);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
