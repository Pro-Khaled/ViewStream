using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Invoice
{
//    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, BaseResponse<InvoiceDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Invoices.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<InvoiceDto>.Fail("Invoice not found");
//                
//                var dto = _mapper.Map<InvoiceDto>(entity);
//                return BaseResponse<InvoiceDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<InvoiceDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
