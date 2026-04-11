using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.Show.UpdateShow
{
    public record UpdateShowCommand(long Id, UpdateShowDto Dto, long UpdatedByUserId) : IRequest<bool>;

}
