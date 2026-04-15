using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Credit
{
    public record GetCreditsByPersonQuery(long PersonId) : IRequest<List<CreditListItemDto>>;

}
