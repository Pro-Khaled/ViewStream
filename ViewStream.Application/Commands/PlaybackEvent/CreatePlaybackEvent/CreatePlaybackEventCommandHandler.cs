using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PlaybackEvent.CreatePlaybackEvent
{
    using PlaybackEvent = ViewStream.Domain.Entities.PlaybackEvent;
    public class CreatePlaybackEventCommandHandler : IRequestHandler<CreatePlaybackEventCommand, PlaybackEventDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePlaybackEventCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PlaybackEventDto> Handle(CreatePlaybackEventCommand request, CancellationToken cancellationToken)
        {
            var evt = _mapper.Map<PlaybackEvent>(request.Dto);
            evt.ProfileId = request.ProfileId;
            evt.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.PlaybackEvents.AddAsync(evt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PlaybackEventDto>(evt);
        }
    }
}
