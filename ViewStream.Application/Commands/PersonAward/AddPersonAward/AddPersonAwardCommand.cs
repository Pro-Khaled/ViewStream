using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.PersonAward.AddPersonAward
{
    public record AddPersonAwardCommand(long PersonId, CreatePersonAwardDto Dto) : IRequest<PersonAwardDto>;

}
