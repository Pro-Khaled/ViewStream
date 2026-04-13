using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Rating.CreateRating
{
    // Upsert: creates or updates a rating for the current profile
    public record UpsertRatingCommand(long ProfileId, CreateUpdateRatingDto Dto) : IRequest<RatingDto>;

}
