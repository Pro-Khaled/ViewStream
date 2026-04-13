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

namespace ViewStream.Application.Queries.UserLibrary
{
    public class GetUserLibraryByTargetQueryHandler : IRequestHandler<GetUserLibraryByTargetQuery, UserLibraryDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserLibraryByTargetQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserLibraryDto?> Handle(GetUserLibraryByTargetQuery request, CancellationToken cancellationToken)
        {
            var items = await _unitOfWork.UserLibraries.FindAsync(
                ul => ul.ProfileId == request.ProfileId && ul.ShowId == request.ShowId && ul.SeasonId == request.SeasonId,
                include: q => q.Include(ul => ul.Profile)
                               .Include(ul => ul.Show)
                               .Include(ul => ul.Season).ThenInclude(s => s.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var item = items.FirstOrDefault();
            return item == null ? null : _mapper.Map<UserLibraryDto>(item);
        }
    }
}
