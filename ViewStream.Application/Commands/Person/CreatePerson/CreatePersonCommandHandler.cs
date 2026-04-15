using AutoMapper;
using MediatR;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.CreatePerson
{
    using Person = ViewStream.Domain.Entities.Person;
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = _mapper.Map<Person>(request.Dto);
            await _unitOfWork.Persons.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<PersonDto>(person);
        }
    }
}
