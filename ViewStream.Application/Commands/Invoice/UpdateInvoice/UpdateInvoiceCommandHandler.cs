using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Invoice.UpdateInvoice
{
//    public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand, BaseResponse<InvoiceDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateInvoiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<InvoiceDto>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Invoices.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<InvoiceDto>.Fail("Invoice not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Invoices.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<InvoiceDto>(entity);
//                // return BaseResponse<InvoiceDto>.Ok(dto, "Invoice updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<InvoiceDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
