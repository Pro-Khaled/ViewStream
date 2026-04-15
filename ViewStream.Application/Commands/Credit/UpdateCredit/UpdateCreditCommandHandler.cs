using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.UpdateCredit
{
    public class UpdateCreditCommandHandler : IRequestHandler<UpdateCreditCommand, CreditDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateCreditCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreditDto?> Handle(UpdateCreditCommand request, CancellationToken cancellationToken)
        {
            var credit = await _unitOfWork.Credits.GetByIdAsync<long>(request.Id, cancellationToken);
            if (credit == null) return null;

            _mapper.Map(request.Dto, credit);
            _unitOfWork.Credits.Update(credit);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
