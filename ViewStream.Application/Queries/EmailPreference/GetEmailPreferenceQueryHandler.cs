using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.EmailPreference
{
    public class GetEmailPreferenceQueryHandler : IRequestHandler<GetEmailPreferenceQuery, EmailPreferenceDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetEmailPreferenceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EmailPreferenceDto?> Handle(GetEmailPreferenceQuery request, CancellationToken cancellationToken)
        {
            var pref = await _unitOfWork.EmailPreferences.GetByIdAsync<long>(request.UserId, cancellationToken);
            return pref == null ? null : _mapper.Map<EmailPreferenceDto>(pref);
        }
    }
}
