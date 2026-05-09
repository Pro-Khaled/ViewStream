using MediatR;

namespace ViewStream.Application.Commands.DataDeletionRequest.DeleteDataDeletionRequest
{
    public record DeleteDataDeletionRequestCommand(long Id, long UserId) : IRequest<bool>;
}
