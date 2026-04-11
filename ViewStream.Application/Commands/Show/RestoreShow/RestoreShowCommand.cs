using MediatR;


namespace ViewStream.Application.Commands.Show.RestoreShow
{
    public record RestoreShowCommand(long Id, long RestoredByUserId) : IRequest<bool>;

}
