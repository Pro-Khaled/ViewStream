using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Commands.UserVector.UpsertUserVector
{
    public record UpsertUserVectorCommand(long ProfileId, CreateUpdateUserVectorDto Dto) : IRequest<UserVectorDto>;

}
