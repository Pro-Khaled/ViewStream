using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    public record UploadShowTrailerCommand(long ShowId, IFormFile TrailerFile, long UploadedByUserId) : IRequest<string>;
}
