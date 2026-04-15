using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Person
{
    public class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, PersonDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPersonByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PersonDto?> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
        {
            var persons = await _unitOfWork.Persons.FindAsync(
                p => p.Id == request.Id,
                include: q => q.Include(p => p.Credits).Include(p => p.PersonAwards),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var person = persons.FirstOrDefault();
            return person == null ? null : _mapper.Map<PersonDto>(person);
        }
    }
}
