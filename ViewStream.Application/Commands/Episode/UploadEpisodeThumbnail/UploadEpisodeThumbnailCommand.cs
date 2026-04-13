using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.Episode.UploadEpisodeThumbnail
{
    public record UploadEpisodeThumbnailCommand(long EpisodeId, IFormFile ThumbnailFile, long UploadedByUserId) : IRequest<string>; // Returns new ThumbnailUrl
}
