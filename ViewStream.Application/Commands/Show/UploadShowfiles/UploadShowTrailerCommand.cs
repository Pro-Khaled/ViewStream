using MediatR;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    public record UploadShowTrailerCommand(long ShowId, IFormFile TrailerFile, long UploadedByUserId)
        : IRequest<string>, IHasUserId
    {
        long? IHasUserId.UserId => UploadedByUserId;
    }
}
