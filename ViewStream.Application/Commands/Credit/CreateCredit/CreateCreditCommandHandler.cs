using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.CreateCredit
{
    using Credit = ViewStream.Domain.Entities.Credit;
    public class CreateCreditCommandHandler : IRequestHandler<CreateCreditCommand, CreditDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCreditCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreditDto> Handle(CreateCreditCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;
            // Validate exactly one target
            int targetCount = (dto.ShowId.HasValue ? 1 : 0) + (dto.SeasonId.HasValue ? 1 : 0) + (dto.EpisodeId.HasValue ? 1 : 0);
            if (targetCount != 1)
                throw new ArgumentException("Exactly one of ShowId, SeasonId, or EpisodeId must be provided.");

            var credit = _mapper.Map<Credit>(dto);
            await _unitOfWork.Credits.AddAsync(credit, cancellationToken);
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
