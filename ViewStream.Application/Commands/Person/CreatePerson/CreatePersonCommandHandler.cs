using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.CreatePerson
{
    using Person = ViewStream.Domain.Entities.Person;
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<CreatePersonCommandHandler> _logger;

        public CreatePersonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<CreatePersonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating person: {PersonName}", request.Dto.Name);

            var person = _mapper.Map<Person>(request.Dto);
            await _unitOfWork.Persons.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Person, object>(
                tableName: "Persons",
                recordId: person.Id,
                action: "INSERT",
                oldValues: null,
                newValues: request.Dto,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Person created with Id: {PersonId}", person.Id);
            return _mapper.Map<PersonDto>(person);
        }
    }
}
