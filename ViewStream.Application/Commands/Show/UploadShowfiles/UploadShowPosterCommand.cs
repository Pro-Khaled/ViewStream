using MediatR;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    public record UploadShowPosterCommand(long ShowId, IFormFile PosterFile, long UploadedByUserId)
        : IRequest<string>, IHasUserId
    {
        long? IHasUserId.UserId => UploadedByUserId;
    }
}
