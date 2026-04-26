using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAward.AddShowAward
{
    using ShowAward = ViewStream.Domain.Entities.ShowAward;
    public class AddShowAwardCommandHandler : IRequestHandler<AddShowAwardCommand, ShowAwardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AddShowAwardCommandHandler> _logger;

        public AddShowAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<AddShowAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<ShowAwardDto> Handle(AddShowAwardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding AwardId: {AwardId} to ShowId: {ShowId}", request.Dto.AwardId, request.ShowId);

            var existing = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == request.ShowId && sa.AwardId == request.Dto.AwardId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("Award already assigned to this show.");

            var showAward = new ShowAward
            {
                ShowId = request.ShowId,
                AwardId = request.Dto.AwardId,
                Won = request.Dto.Won
            };

            await _unitOfWork.ShowAwards.AddAsync(showAward, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<ShowAward, object>(
                tableName: "ShowAwards",
                recordId: request.ShowId.GetHashCode() ^ request.Dto.AwardId,
                action: "INSERT",
                oldValues: null,
                newValues: new { request.ShowId, request.Dto.AwardId, request.Dto.Won },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Award {AwardId} assigned to Show {ShowId}", request.Dto.AwardId, request.ShowId);

            var result = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == showAward.ShowId && sa.AwardId == showAward.AwardId,
                include: q => q.Include(sa => sa.Show).Include(sa => sa.Award),
                cancellationToken: cancellationToken);

            return _mapper.Map<ShowAwardDto>(result.First());
        }
    }
}
