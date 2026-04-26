using MediatR;
using Microsoft.AspNetCore.Http;
using ViewStream.Application.Behaviors;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    public record UploadShowBackdropCommand(long ShowId, IFormFile BackdropFile, long UploadedByUserId)
        : IRequest<string>, IHasUserId
    {
        long? IHasUserId.UserId => UploadedByUserId;
    }
}
