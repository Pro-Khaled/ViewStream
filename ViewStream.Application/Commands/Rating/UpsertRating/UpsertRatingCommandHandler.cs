using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Rating.CreateRating
{
    using Rating = ViewStream.Domain.Entities.Rating;
    public class UpsertRatingCommandHandler : IRequestHandler<UpsertRatingCommand, RatingDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpsertRatingCommandHandler> _logger;

        public UpsertRatingCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpsertRatingCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<RatingDto> Handle(UpsertRatingCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Upserting rating for ShowId: {ShowId}, ProfileId: {ProfileId}",
                request.Dto.ShowId, request.ProfileId);

            var existing = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == request.ProfileId && r.ShowId == request.Dto.ShowId,
                cancellationToken: cancellationToken);

            var rating = existing.FirstOrDefault();
            bool isNew = rating == null;
            short oldRating = rating?.Rating1 ?? 0;
            string action = isNew ? "INSERT" : "UPDATE";

            if (isNew)
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

            _auditContext.SetAudit<Rating, object>(
                tableName: "Ratings",
                recordId: request.ProfileId.GetHashCode() ^ request.Dto.ShowId.GetHashCode(),
                action: action,
                oldValues: isNew ? null : new { ProfileId = request.ProfileId, ShowId = request.Dto.ShowId, Rating = oldRating },
                newValues: new { request.ProfileId, request.Dto.ShowId, Rating = request.Dto.Rating },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Rating {Action} for ShowId: {ShowId}, ProfileId: {ProfileId}",
                action, request.Dto.ShowId, request.ProfileId);

            var result = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == rating.ProfileId && r.ShowId == rating.ShowId,
                include: q => q.Include(r => r.Profile).Include(r => r.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<RatingDto>(result.First());
        }
    }
}
