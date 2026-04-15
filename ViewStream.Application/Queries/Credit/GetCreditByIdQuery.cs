using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Credit
{
    public record GetCreditByIdQuery(long Id) : IRequest<CreditDto?>;

}
