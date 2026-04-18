using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow
{
    using PersonalizedRow = ViewStream.Domain.Entities.PersonalizedRow;
    public class UpsertPersonalizedRowCommandHandler : IRequestHandler<UpsertPersonalizedRowCommand, PersonalizedRowDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpsertPersonalizedRowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PersonalizedRowDto> Handle(UpsertPersonalizedRowCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.PersonalizedRows.FindAsync(
                r => r.ProfileId == request.ProfileId && r.RowName == request.Dto.RowName,
                cancellationToken: cancellationToken);

            var row = existing.FirstOrDefault();
            var showIdsJson = JsonSerializer.Serialize(request.Dto.ShowIds);

            if (row == null)
            {
                row = new PersonalizedRow
                {
                    ProfileId = request.ProfileId,
                    RowName = request.Dto.RowName,
                    ShowIdsJson = showIdsJson,
                    GeneratedAt = DateTime.UtcNow
                };
                await _unitOfWork.PersonalizedRows.AddAsync(row, cancellationToken);
            }
            else
            {
                row.ShowIdsJson = showIdsJson;
                row.GeneratedAt = DateTime.UtcNow;
                _unitOfWork.PersonalizedRows.Update(row);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PersonalizedRowDto
            {
                ProfileId = row.ProfileId,
                RowName = row.RowName,
                ShowIds = request.Dto.ShowIds,
                GeneratedAt = row.GeneratedAt
            };
        }
    }
}
