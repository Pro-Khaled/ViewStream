using MediatR;

namespace ViewStream.Application.Queries.Invoice.GetInvoicePdf
{
    public sealed record GetInvoicePdfResult(byte[] Content, string FileName);

    // Admin override: invoice id only (controller uses new GetInvoicePdfQuery(id))
    // User endpoint: controller uses GetInvoicePdfQuery(id, userId)
    public sealed record GetInvoicePdfQuery(long Id, long? UserId = null) : IRequest<GetInvoicePdfResult?>;
}
