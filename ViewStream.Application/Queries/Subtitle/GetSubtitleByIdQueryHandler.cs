using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Subtitle
{
    public class GetSubtitleByIdQueryHandler : IRequestHandler<GetSubtitleByIdQuery, SubtitleDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSubtitleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubtitleDto?> Handle(GetSubtitleByIdQuery request, CancellationToken cancellationToken)
        {
            var subtitles = await _unitOfWork.Subtitles.FindAsync(
                predicate: s => s.Id == request.Id && s.IsDeleted != true,
                include: q => q.Include(s => s.Episode),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var subtitle = subtitles.FirstOrDefault();
            return subtitle == null ? null : _mapper.Map<SubtitleDto>(subtitle);
        }
    }
}
