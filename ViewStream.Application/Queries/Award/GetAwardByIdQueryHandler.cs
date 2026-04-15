using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Award
{
    public class GetAwardByIdQueryHandler : IRequestHandler<GetAwardByIdQuery, AwardDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAwardByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<AwardDto?> Handle(GetAwardByIdQuery request, CancellationToken cancellationToken)
        {
            var awards = await _unitOfWork.Awards.FindAsync(
                a => a.Id == request.Id,
                include: q => q.Include(a => a.PersonAwards).Include(a => a.ShowAwards),
                asNoTracking: true, cancellationToken: cancellationToken);
            var award = awards.FirstOrDefault();
            return award == null ? null : _mapper.Map<AwardDto>(award);
        }
    }
}
