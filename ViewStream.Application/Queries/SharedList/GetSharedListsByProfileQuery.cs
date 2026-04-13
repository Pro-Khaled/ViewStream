using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.SharedList
{
    public record GetSharedListsByProfileQuery(long ProfileId, bool IncludePrivate = false) : IRequest<List<SharedListListItemDto>>;
}
