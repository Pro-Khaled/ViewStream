using AutoMapper;
using Microsoft.Extensions.Logging;
using ViewStream.Application.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ViewStream.Application.Exceptions;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PlaybackEvent.DeletePlaybackEvent
{
    public class DeletePlaybackEventCommandHandler : IRequestHandler<DeletePlaybackEventCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePlaybackEventCommandHandler> _logger;

        public DeletePlaybackEventCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeletePlaybackEventCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePlaybackEventCommand request, CancellationToken cancellationToken)
        {
            var playbackEvent = await _unitOfWork.PlaybackEvents.GetByIdAsync<long>(request.Id, cancellationToken);
            if (playbackEvent == null)
            {
                throw new NotFoundException("PlaybackEvent", request.Id);
            }

            _unitOfWork.PlaybackEvents.Delete(playbackEvent);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Playback event hard-deleted with Id: {EventId}", playbackEvent.Id);
            return true;
        }
    }
}

