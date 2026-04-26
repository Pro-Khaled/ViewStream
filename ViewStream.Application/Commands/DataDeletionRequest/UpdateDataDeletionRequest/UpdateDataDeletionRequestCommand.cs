using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.DataDeletionRequest.UpdateDataDeletionRequest
{
    public record UpdateDataDeletionRequestCommand(long Id, UpdateDataDeletionRequestDto Dto, long ActorUserId) : IRequest<DataDeletionRequestDto?>;

}
