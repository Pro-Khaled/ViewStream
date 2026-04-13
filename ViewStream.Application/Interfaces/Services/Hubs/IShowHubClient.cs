using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Interfaces.Services.Hubs
{
    public interface IShowHubClient
    {
        Task SendShowPosterUpdatedAsync(ShowDto show, CancellationToken cancellationToken = default);
        Task SendShowBackdropUpdatedAsync(ShowDto show, CancellationToken cancellationToken = default);
        Task SendShowTrailerUpdatedAsync(ShowDto show, CancellationToken cancellationToken = default);
    }
}
