using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.UpdatePerson
{
    using Person = ViewStream.Domain.Entities.Person;
    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, PersonDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<UpdatePersonCommandHandler> _logger;

        public UpdatePersonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<UpdatePersonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PersonDto?> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating person Id: {PersonId}", request.Id);

            var person = await _unitOfWork.Persons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (person == null)
            {
                _logger.LogWarning("Person not found. Id: {PersonId}", request.Id);
                return null;
            }

            var oldValues = _mapper.Map<PersonDto>(person);
            _mapper.Map(request.Dto, person);
            _unitOfWork.Persons.Update(person);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Person, object>(
                tableName: "Persons",
                recordId: person.Id,
                action: "UPDATE",
                oldValues: oldValues,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Person updated. Id: {PersonId}", person.Id);
            return _mapper.Map<PersonDto>(person);
        }
    }
}
