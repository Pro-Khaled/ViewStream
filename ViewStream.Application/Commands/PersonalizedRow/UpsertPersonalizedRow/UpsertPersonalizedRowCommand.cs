using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PersonalizedRow.UpsertPersonalizedRow
{
    public record UpsertPersonalizedRowCommand(long ProfileId, CreateUpdatePersonalizedRowDto Dto) : IRequest<PersonalizedRowDto>;

}
