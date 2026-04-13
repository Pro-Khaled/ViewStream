using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.UserInteraction
{
    public class GetUserInteractionByIdQueryHandler : IRequestHandler<GetUserInteractionByIdQuery, UserInteractionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserInteractionByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserInteractionDto?> Handle(GetUserInteractionByIdQuery request, CancellationToken cancellationToken)
        {
            var interactions = await _unitOfWork.UserInteractions.FindAsync(
                i => i.Id == request.Id,
                include: q => q.Include(i => i.Profile).Include(i => i.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var interaction = interactions.FirstOrDefault();
            return interaction == null ? null : _mapper.Map<UserInteractionDto>(interaction);
        }
    }
}
