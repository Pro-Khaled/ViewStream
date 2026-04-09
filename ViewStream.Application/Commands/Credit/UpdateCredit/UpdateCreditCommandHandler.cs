using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Credit.UpdateCredit
{
//    public class UpdateCreditCommandHandler : IRequestHandler<UpdateCreditCommand, BaseResponse<CreditDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateCreditCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CreditDto>> Handle(UpdateCreditCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Credits.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CreditDto>.Fail("Credit not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Credits.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<CreditDto>(entity);
//                // return BaseResponse<CreditDto>.Ok(dto, "Credit updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CreditDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
