using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonAward.AddPersonAward
{
    using PersonAward = ViewStream.Domain.Entities.PersonAward;
    public class AddPersonAwardCommandHandler : IRequestHandler<AddPersonAwardCommand, PersonAwardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<AddPersonAwardCommandHandler> _logger;

        public AddPersonAwardCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<AddPersonAwardCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PersonAwardDto> Handle(AddPersonAwardCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding AwardId: {AwardId} to PersonId: {PersonId}", request.Dto.AwardId, request.PersonId);

            var existing = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == request.PersonId && pa.AwardId == request.Dto.AwardId,
                cancellationToken: cancellationToken);

            if (existing.Any())
                throw new InvalidOperationException("Award already assigned to this person.");

            var personAward = new PersonAward
            {
                PersonId = request.PersonId,
                AwardId = request.Dto.AwardId,
                Won = request.Dto.Won
            };

            await _unitOfWork.PersonAwards.AddAsync(personAward, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<PersonAward, object>(
                tableName: "PersonAwards",
                recordId: request.PersonId.GetHashCode() ^ request.Dto.AwardId,
                action: "INSERT",
                oldValues: null,
                newValues: new { request.PersonId, request.Dto.AwardId, request.Dto.Won },
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Award {AwardId} assigned to Person {PersonId}", request.Dto.AwardId, request.PersonId);

            var result = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == personAward.PersonId && pa.AwardId == personAward.AwardId,
                include: q => q.Include(pa => pa.Person).Include(pa => pa.Award),
                cancellationToken: cancellationToken);

            return _mapper.Map<PersonAwardDto>(result.First());
        }
    }
}
