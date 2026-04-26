using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.CreateCredit
{
    using Credit = ViewStream.Domain.Entities.Credit;
    public class CreateCreditCommandHandler : IRequestHandler<CreateCreditCommand, CreditDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreateCreditCommandHandler> _logger;

        public CreateCreditCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreateCreditCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<CreditDto> Handle(CreateCreditCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            int targetCount = (dto.ShowId.HasValue ? 1 : 0) + (dto.SeasonId.HasValue ? 1 : 0) + (dto.EpisodeId.HasValue ? 1 : 0);
            if (targetCount != 1)
                throw new ArgumentException("Exactly one of ShowId, SeasonId, or EpisodeId must be provided.");

            _logger.LogInformation("Creating credit for PersonId: {PersonId}, Role: {Role}, ShowId: {ShowId}, SeasonId: {SeasonId}, EpisodeId: {EpisodeId}",
                dto.PersonId, dto.Role, dto.ShowId, dto.SeasonId, dto.EpisodeId);

            var credit = _mapper.Map<Credit>(dto);
            await _unitOfWork.Credits.AddAsync(credit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Credit, object>(
                tableName: "Credits",
                recordId: credit.Id,
                action: "INSERT",
                oldValues: null,
                newValues: dto,
                changedByUserId: request.UserId
            );

            _logger.LogInformation("Credit created with Id: {CreditId}", credit.Id);

            var result = await _unitOfWork.Credits.FindAsync(
                c => c.Id == credit.Id,
                include: q => q.Include(c => c.Person)
                               .Include(c => c.Show)
                               .Include(c => c.Season).ThenInclude(s => s.Show)
                               .Include(c => c.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Show),
                cancellationToken: cancellationToken);
            return _mapper.Map<CreditDto>(result.First());
        }
    }
}
