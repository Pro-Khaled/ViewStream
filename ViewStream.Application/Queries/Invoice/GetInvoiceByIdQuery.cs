using MediatR;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Invoice
{
    public record GetInvoiceByIdQuery(long Id, long? UserId = null) : IRequest<InvoiceDto?>; // UserId optional for admin

}
