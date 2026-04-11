using MediatR;
using ViewStream.Application.Common;

namespace ViewStream.Application.Commands.Show.DeleteShow
{
    public record DeleteShowCommand(long Id, long DeletedByUserId) : IRequest<bool>; // Soft delete

}
