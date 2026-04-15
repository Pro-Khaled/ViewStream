using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonAward.AddPersonAward
{
    using PersonAward = ViewStream.Domain.Entities.PersonAward;
    public class AddPersonAwardCommandHandler : IRequestHandler<AddPersonAwardCommand, PersonAwardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddPersonAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PersonAwardDto> Handle(AddPersonAwardCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == request.PersonId && pa.AwardId == request.Dto.AwardId, cancellationToken: cancellationToken);
            if (existing.Any()) throw new InvalidOperationException("Award already assigned to this person.");
            var personAward = new PersonAward
            {
                PersonId = request.PersonId,
                AwardId = request.Dto.AwardId,
                Won = request.Dto.Won
            };
            await _unitOfWork.PersonAwards.AddAsync(personAward, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = await _unitOfWork.PersonAwards.FindAsync(
                pa => pa.PersonId == personAward.PersonId && pa.AwardId == personAward.AwardId,
                include: q => q.Include(pa => pa.Person).Include(pa => pa.Award), cancellationToken: cancellationToken);
            return _mapper.Map<PersonAwardDto>(result.First());
        }
    }
}
