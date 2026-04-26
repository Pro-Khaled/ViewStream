using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PushToken
{
    public class GetUserPushTokensQueryHandler : IRequestHandler<GetUserPushTokensQuery, List<PushTokenDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserPushTokensQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PushTokenDto>> Handle(GetUserPushTokensQuery request, CancellationToken cancellationToken)
        {
            var tokens = await _unitOfWork.PushTokens.FindAsync(
                pt => pt.UserId == request.UserId,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<PushTokenDto>>(tokens);
        }
    }
}
