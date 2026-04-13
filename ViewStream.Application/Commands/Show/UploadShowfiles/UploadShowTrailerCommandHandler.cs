using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Application.Interfaces.Services.Hubs;
using ViewStream.Application.Queries.Show;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.UploadShowfiles
{
    public class UploadShowTrailerCommandHandler : IRequestHandler<UploadShowTrailerCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;
        private readonly IMediator _mediator;
        private readonly IShowHubClient _hubClient;

        public UploadShowTrailerCommandHandler(
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorage,
            IMediator mediator,
            IShowHubClient hubClient)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _mediator = mediator;
            _hubClient = hubClient;
        }

        public async Task<string> Handle(UploadShowTrailerCommand request, CancellationToken cancellationToken)
        {
            var show = await _unitOfWork.Shows.GetByIdAsync<long>(request.ShowId, cancellationToken);
            if (show == null) throw new InvalidOperationException("Show not found.");

            if (!string.IsNullOrEmpty(show.TrailerUrl))
                _fileStorage.DeleteFile(show.TrailerUrl);

            var trailerUrl = await _fileStorage.SaveTrailerAsync(request.TrailerFile, request.ShowId, cancellationToken);
            show.TrailerUrl = trailerUrl;
            show.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Shows.Update(show);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedShowDto = await _mediator.Send(new GetShowByIdQuery(request.ShowId), cancellationToken);
            await _hubClient.SendShowTrailerUpdatedAsync(updatedShowDto, cancellationToken);

            return trailerUrl;
        }
    }
}
