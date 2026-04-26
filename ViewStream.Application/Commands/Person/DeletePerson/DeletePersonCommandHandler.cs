using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ViewStream.Application.DTOs;
using ViewStream.Application.Helpers;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.DeletePerson
{
    using Person = ViewStream.Domain.Entities.Person;
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuditContext _auditContext;
        private readonly ILogger<DeletePersonCommandHandler> _logger;

        public DeletePersonCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuditContext auditContext,
            ILogger<DeletePersonCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting person Id: {PersonId}", request.Id);

            var person = await _unitOfWork.Persons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (person == null)
            {
                _logger.LogWarning("Person not found. Id: {PersonId}", request.Id);
                return false;
            }

            var oldValues = _mapper.Map<PersonDto>(person);
            _unitOfWork.Persons.Delete(person);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _auditContext.SetAudit<Person, object>(
                tableName: "Persons",
                recordId: request.Id,
                action: "DELETE",
                oldValues: oldValues,
                changedByUserId: request.ActorUserId
            );

            _logger.LogInformation("Person deleted. Id: {PersonId}", request.Id);
            return true;
        }
    }
}
