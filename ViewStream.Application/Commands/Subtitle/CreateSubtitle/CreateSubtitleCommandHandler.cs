using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.CreateSubtitle
{
    using Subtitle = Domain.Entities.Subtitle;
    public class CreateSubtitleCommandHandler : IRequestHandler<CreateSubtitleCommand, long>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSubtitleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<long> Handle(CreateSubtitleCommand request, CancellationToken cancellationToken)
        {
            var subtitle = _mapper.Map<Subtitle>(request.Dto);
            subtitle.CreatedAt = DateTime.UtcNow;
            subtitle.IsDeleted = false;

            await _unitOfWork.Subtitles.AddAsync(subtitle, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return subtitle.Id;
        }
    }
}
