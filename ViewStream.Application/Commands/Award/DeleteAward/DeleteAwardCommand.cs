using MediatR;

namespace ViewStream.Application.Commands.Award.DeleteAward
{
    public record DeleteAwardCommand(int Id) : IRequest<bool>;

}
