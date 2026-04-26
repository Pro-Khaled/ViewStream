using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.DataDeletionRequest
{
    public record GetDataDeletionRequestByIdQuery(long Id) : IRequest<DataDeletionRequestDto?>;

}
