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

namespace ViewStream.Application.Commands.ItemVector.UpsertItemVector
{
    using ItemVector = Domain.Entities.ItemVector;
    public class UpsertItemVectorCommandHandler : IRequestHandler<UpsertItemVectorCommand, ItemVectorDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpsertItemVectorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ItemVectorDto> Handle(UpsertItemVectorCommand request, CancellationToken cancellationToken)
        {
            var vector = await _unitOfWork.ItemVectors.GetByIdAsync<long>(request.ShowId, cancellationToken);

            if (vector == null)
            {
                vector = new ItemVector
                {
                    ShowId = request.ShowId,
                    EmbeddingJson = request.Dto.EmbeddingJson,
                    LastUpdated = DateTime.UtcNow
                };
                await _unitOfWork.ItemVectors.AddAsync(vector, cancellationToken);
            }
            else
            {
                vector.EmbeddingJson = request.Dto.EmbeddingJson;
                vector.LastUpdated = DateTime.UtcNow;
                _unitOfWork.ItemVectors.Update(vector);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.ItemVectors.FindAsync(
                v => v.ShowId == request.ShowId,
                include: q => q.Include(v => v.Show),
                cancellationToken: cancellationToken);

            return _mapper.Map<ItemVectorDto>(result.First());
        }
    }
}
