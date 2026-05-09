using MediatR;

namespace ViewStream.Application.Commands.PersonalizedRow.RegenerateRecommendations
{
    public record RegenerateRecommendationsCommand(long ProfileId, long ActorUserId) : IRequest<bool>;
}
