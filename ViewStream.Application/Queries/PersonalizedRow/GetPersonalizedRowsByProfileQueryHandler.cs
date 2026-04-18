using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.PersonalizedRow
{
    public class GetPersonalizedRowsByProfileQueryHandler : IRequestHandler<GetPersonalizedRowsByProfileQuery, List<PersonalizedRowDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPersonalizedRowsByProfileQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<List<PersonalizedRowDto>> Handle(GetPersonalizedRowsByProfileQuery request, CancellationToken cancellationToken)
        {
            var rows = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId,
                asNoTracking: true,
                cancellationToken: cancellationToken);

            return rows.Select(r => new PersonalizedRowDto
            {
                ProfileId = r.ProfileId,
                RowName = r.RowName,
                ShowIds = string.IsNullOrEmpty(r.ShowIdsJson)
                    ? new List<long>()
                    : JsonSerializer.Deserialize<List<long>>(r.ShowIdsJson) ?? new List<long>(),
                GeneratedAt = r.GeneratedAt
            }).OrderBy(r => r.RowName).ToList();
        }
    }
}
