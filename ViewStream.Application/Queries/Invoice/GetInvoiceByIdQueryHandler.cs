using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ViewStream.Application.Common;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Invoice
{
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _unitOfWork.Invoices.FindAsync(
                i => i.Id == request.Id,
                include: q => q.Include(i => i.User).Include(i => i.Subscription),
                asNoTracking: true, cancellationToken: cancellationToken);
            var invoice = invoices.FirstOrDefault();
            if (invoice == null) return null;
            if (request.UserId.HasValue && invoice.UserId != request.UserId) return null; // Basic security
            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
