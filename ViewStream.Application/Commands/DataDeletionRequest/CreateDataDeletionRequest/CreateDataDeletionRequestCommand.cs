using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.DataDeletionRequest.CreateDataDeletionRequest
{
    public record CreateDataDeletionRequestCommand(long UserId) : IRequest<DataDeletionRequestDto>;
}
