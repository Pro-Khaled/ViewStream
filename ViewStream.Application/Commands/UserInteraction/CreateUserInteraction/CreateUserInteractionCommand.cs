using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserInteraction.CreateUserInteraction
{
    public record CreateUserInteractionCommand(long ProfileId, CreateUserInteractionDto Dto) : IRequest<UserInteractionDto>;

}
