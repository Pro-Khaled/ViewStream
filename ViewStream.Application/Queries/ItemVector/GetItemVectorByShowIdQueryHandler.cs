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

namespace ViewStream.Application.Queries.ItemVector
{
    public class GetItemVectorByShowIdQueryHandler : IRequestHandler<GetItemVectorByShowIdQuery, ItemVectorDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetItemVectorByShowIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ItemVectorDto?> Handle(GetItemVectorByShowIdQuery request, CancellationToken cancellationToken)
        {
            var vectors = await _unitOfWork.ItemVectors.FindAsync(
                v => v.ShowId == request.ShowId,
                include: q => q.Include(v => v.Show),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var vector = vectors.FirstOrDefault();
            return vector == null ? null : _mapper.Map<ItemVectorDto>(vector);
        }
    }
}
