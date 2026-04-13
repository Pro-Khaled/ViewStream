using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.UserInteraction
{
    public record GetUserInteractionByIdQuery(long Id) : IRequest<UserInteractionDto?>;

}
