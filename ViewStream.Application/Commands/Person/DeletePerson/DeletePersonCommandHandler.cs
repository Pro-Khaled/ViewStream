using MediatR;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.DeletePerson
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePersonCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _unitOfWork.Persons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (person == null) return false;

            _unitOfWork.Persons.Delete(person);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
