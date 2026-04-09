using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAvailability.UpdateShowAvailability
{
//    public class UpdateShowAvailabilityCommandHandler : IRequestHandler<UpdateShowAvailabilityCommand, BaseResponse<ShowAvailabilityDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateShowAvailabilityCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ShowAvailabilityDto>> Handle(UpdateShowAvailabilityCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ShowAvailabilitys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ShowAvailabilityDto>.Fail("ShowAvailability not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.ShowAvailabilitys.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ShowAvailabilityDto>(entity);
//                // return BaseResponse<ShowAvailabilityDto>.Ok(dto, "ShowAvailability updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ShowAvailabilityDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
