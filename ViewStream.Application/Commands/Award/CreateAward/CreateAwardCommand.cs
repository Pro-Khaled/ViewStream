using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Award.CreateAward
{
    public record CreateAwardCommand(CreateAwardDto Dto) : IRequest<AwardDto>;

}
