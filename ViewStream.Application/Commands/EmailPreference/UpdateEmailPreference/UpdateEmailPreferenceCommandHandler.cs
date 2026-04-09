using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.EmailPreference.UpdateEmailPreference
{
//    public class UpdateEmailPreferenceCommandHandler : IRequestHandler<UpdateEmailPreferenceCommand, BaseResponse<EmailPreferenceDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateEmailPreferenceCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<EmailPreferenceDto>> Handle(UpdateEmailPreferenceCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.EmailPreferences.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<EmailPreferenceDto>.Fail("EmailPreference not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.EmailPreferences.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<EmailPreferenceDto>(entity);
//                // return BaseResponse<EmailPreferenceDto>.Ok(dto, "EmailPreference updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<EmailPreferenceDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
