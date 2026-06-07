using FluentValidation;

namespace ViewStream.Application.Commands.Episode.CreateEpisode;

public class CreateEpisodeCommandValidator : AbstractValidator<CreateEpisodeCommand>
{
    public CreateEpisodeCommandValidator()
    {
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Dto.SeasonId).GreaterThan(0);
        RuleFor(x => x.Dto.EpisodeNumber).GreaterThan((short)0);
        RuleFor(x => x.Dto.RuntimeSeconds).GreaterThan(0).When(x => x.Dto.RuntimeSeconds.HasValue);
    }
}
