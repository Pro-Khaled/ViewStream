using MediatR;

namespace ViewStream.Application.Commands.Rating.DeleteRating
{
    public record DeleteRatingCommand(long ProfileId, long ShowId) : IRequest<bool>;

}
