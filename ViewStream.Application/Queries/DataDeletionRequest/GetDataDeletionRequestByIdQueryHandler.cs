using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.DataDeletionRequest
{
    public class GetDataDeletionRequestByIdQueryHandler : IRequestHandler<GetDataDeletionRequestByIdQuery, DataDeletionRequestDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDataDeletionRequestByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DataDeletionRequestDto?> Handle(GetDataDeletionRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var req = await _unitOfWork.DataDeletionRequests.FindAsync(
                r => r.Id == request.Id,
                include: q => q.Include(r => r.User),
                asNoTracking: true,
                cancellationToken: cancellationToken);

            var entity = req.FirstOrDefault();
            return entity == null ? null : _mapper.Map<DataDeletionRequestDto>(entity);
        }
    }
}
