using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent
{
    using PlaybackEvent = ViewStream.Domain.Entities.PlaybackEvent;
    public class CreatePlaybackEventCommandHandler : IRequestHandler<CreatePlaybackEventCommand, PlaybackEventDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreatePlaybackEventCommandHandler> _logger;

        public CreatePlaybackEventCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreatePlaybackEventCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PlaybackEventDto> Handle(CreatePlaybackEventCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Logging playback event for ProfileId: {ProfileId}, EpisodeId: {EpisodeId}, EventType: {EventType}",
                request.ProfileId, request.Dto.EpisodeId, request.Dto.EventType);

            var evt = _mapper.Map<PlaybackEvent>(request.Dto);
            evt.ProfileId = request.ProfileId;
            evt.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.PlaybackEvents.AddAsync(evt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Playback event logged with Id: {EventId}", evt.Id);

            return _mapper.Map<PlaybackEventDto>(evt);
        }
    }
}
