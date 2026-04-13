using MediatR;
using AutoMapper;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Subtitle.UpdateSubtitle
{
    public class UpdateSubtitleCommandHandler : IRequestHandler<UpdateSubtitleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSubtitleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateSubtitleCommand request, CancellationToken cancellationToken)
        {
            var subtitle = await _unitOfWork.Subtitles.GetByIdAsync<long>(request.Id, cancellationToken);
            if (subtitle == null || subtitle.IsDeleted == true)
                return false;

            _mapper.Map(request.Dto, subtitle);
            subtitle.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Subtitles.Update(subtitle);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
