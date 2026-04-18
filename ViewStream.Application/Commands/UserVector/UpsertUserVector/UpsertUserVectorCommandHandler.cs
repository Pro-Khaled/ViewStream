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

namespace ViewStream.Application.Commands.UserVector.UpsertUserVector
{
    using UserVector = ViewStream.Domain.Entities.UserVector;
    public class UpsertUserVectorCommandHandler : IRequestHandler<UpsertUserVectorCommand, UserVectorDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpsertUserVectorCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserVectorDto> Handle(UpsertUserVectorCommand request, CancellationToken cancellationToken)
        {
            var vector = await _unitOfWork.UserVectors.GetByIdAsync<long>(request.ProfileId, cancellationToken);

            if (vector == null)
            {
                vector = new UserVector
                {
                    ProfileId = request.ProfileId,
                    EmbeddingJson = request.Dto.EmbeddingJson,
                    LastUpdated = DateTime.UtcNow
                };
                await _unitOfWork.UserVectors.AddAsync(vector, cancellationToken);
            }
            else
            {
                vector.EmbeddingJson = request.Dto.EmbeddingJson;
                vector.LastUpdated = DateTime.UtcNow;
                _unitOfWork.UserVectors.Update(vector);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = await _unitOfWork.UserVectors.FindAsync(
                v => v.ProfileId == request.ProfileId,
                include: q => q.Include(v => v.Profile),
                cancellationToken: cancellationToken);

            return _mapper.Map<UserVectorDto>(result.First());
        }
    }
}
