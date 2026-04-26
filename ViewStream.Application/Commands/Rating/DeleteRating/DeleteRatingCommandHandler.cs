using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Rating.DeleteRating
{
    using Rating = ViewStream.Domain.Entities.Rating;
    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeleteRatingCommandHandler> _logger;

        public DeleteRatingCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeleteRatingCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting rating for ShowId: {ShowId}, ProfileId: {ProfileId}",
                request.ShowId, request.ProfileId);

            var ratings = await _unitOfWork.Ratings.FindAsync(
                r => r.ProfileId == request.ProfileId && r.ShowId == request.ShowId,
                cancellationToken: cancellationToken);

            var rating = ratings.FirstOrDefault();
            if (rating == null)
            {
                _logger.LogWarning("Rating not found. ShowId: {ShowId}, ProfileId: {ProfileId}",
                    request.ShowId, request.ProfileId);
                return false;
            }

            var oldValues = _mapper.Map<RatingDto>(rating);
            _unitOfWork.Ratings.Delete(rating);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Rating, object>(
                tableName: "Ratings",
                recordId: request.ProfileId.GetHashCode() ^ request.ShowId,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Rating deleted for ShowId: {ShowId}, ProfileId: {ProfileId}",
                request.ShowId, request.ProfileId);
            return true;
        }
    }
}
