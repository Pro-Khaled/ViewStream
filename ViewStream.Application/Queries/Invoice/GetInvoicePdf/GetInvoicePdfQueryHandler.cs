using MediatR;
using ViewStream.Application.DTOs;

namespace ViewStream.Application.Queries.Invoice.GetInvoicePdf
{
    // Minimal stub so ViewStream.API compiles.
    // TODO: Implement real invoice PDF generation/retrieval and storage wiring.
    public class GetInvoicePdfQueryHandler : IRequestHandler<GetInvoicePdfQuery, GetInvoicePdfResult?>
    {
        public Task<GetInvoicePdfResult?> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
        {
            // Returning null makes the API return 404 instead of failing at build/runtime.
            return Task.FromResult<GetInvoicePdfResult?>(null);
        }
    }
}
