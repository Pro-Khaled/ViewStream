using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Profile.UpdateProfile
{
//    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, BaseResponse<ProfileDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Profiles.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ProfileDto>.Fail("Profile not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Profiles.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ProfileDto>(entity);
//                // return BaseResponse<ProfileDto>.Ok(dto, "Profile updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ProfileDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
