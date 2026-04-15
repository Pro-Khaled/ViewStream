using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Award.UpdateAward
{
    public record UpdateAwardCommand(int Id, UpdateAwardDto Dto) : IRequest<AwardDto?>;

}
