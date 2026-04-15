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

namespace ViewStream.Application.Commands.ShowAward.AddShowAward
{
    using ShowAward = ViewStream.Domain.Entities.ShowAward;
    public class AddShowAwardCommandHandler : IRequestHandler<AddShowAwardCommand, ShowAwardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AddShowAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<ShowAwardDto> Handle(AddShowAwardCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == request.ShowId && sa.AwardId == request.Dto.AwardId, cancellationToken: cancellationToken);
            if (existing.Any()) throw new InvalidOperationException("Award already assigned to this show.");
            var showAward = new ShowAward
            {
                ShowId = request.ShowId,
                AwardId = request.Dto.AwardId,
                Won = request.Dto.Won
            };
            await _unitOfWork.ShowAwards.AddAsync(showAward, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = await _unitOfWork.ShowAwards.FindAsync(
                sa => sa.ShowId == showAward.ShowId && sa.AwardId == showAward.AwardId,
                include: q => q.Include(sa => sa.Show).Include(sa => sa.Award), cancellationToken: cancellationToken);
            return _mapper.Map<ShowAwardDto>(result.First());
        }
    }
}
