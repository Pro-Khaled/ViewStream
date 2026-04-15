using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.UpdatePerson
{
    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, PersonDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PersonDto?> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _unitOfWork.Persons.GetByIdAsync<long>(request.Id, cancellationToken);
            if (person == null) return null;

            _mapper.Map(request.Dto, person);
            _unitOfWork.Persons.Update(person);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PersonDto>(person);
        }
    }
}
