using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Rating.CreateRating
{
    using Rating = ViewStream.Domain.Entities.Rating;
    public class UpsertRatingCommandHandler : IRequestHandler<UpsertRatingCommand, RatingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpsertRatingCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<RatingDto> Handle(UpsertRatingCommand request, CancellationToken cancellationToken)
        {
            // Check if rating already exists
            var existing = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == request.ProfileId && r.ShowId == request.Dto.ShowId,
                cancellationToken: cancellationToken);

            var rating = existing.FirstOrDefault();

            if (rating == null)
            {
                rating = new Rating
                {
                    ProfileId = request.ProfileId,
                    ShowId = request.Dto.ShowId,
                    Rating1 = request.Dto.Rating,
                    RatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Ratings.AddAsync(rating, cancellationToken);
            }
            else
            {
                rating.Rating1 = request.Dto.Rating;
                rating.RatedAt = DateTime.UtcNow;
                _unitOfWork.Ratings.Update(rating);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load navigation properties for DTO
            var result = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == rating.ProfileId && r.ShowId == rating.ShowId,
                include: q => q.Include(r => r.Profile).Include(r => r.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<RatingDto>(result.First());
        }
    }
}
