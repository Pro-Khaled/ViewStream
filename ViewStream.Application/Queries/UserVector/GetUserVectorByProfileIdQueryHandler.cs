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

namespace ViewStream.Application.Queries.UserVector
{
    public class GetUserVectorByProfileIdQueryHandler : IRequestHandler<GetUserVectorByProfileIdQuery, UserVectorDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserVectorByProfileIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserVectorDto?> Handle(GetUserVectorByProfileIdQuery request, CancellationToken cancellationToken)
        {
            var vectors = await _unitOfWork.UserVectors.FindAsync(
                v => v.ProfileId == request.ProfileId,
                include: q => q.Include(v => v.Profile),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var vector = vectors.FirstOrDefault();
            return vector == null ? null : _mapper.Map<UserVectorDto>(vector);
        }
    }
}
