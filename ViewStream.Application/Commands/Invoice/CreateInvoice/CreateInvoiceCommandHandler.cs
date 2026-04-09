using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Invoice.CreateInvoice
{
  //  public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, BaseResponse<InvoiceDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateInvoiceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<Invoice>(request);
  //              
  //              // await _unitOfWork.Invoices.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<InvoiceDto>(entity);
  //              // return BaseResponse<InvoiceDto>.Ok(dto, "Invoice created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<InvoiceDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
