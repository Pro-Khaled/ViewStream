using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Device
{
//    public class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, BaseResponse<DeviceDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public GetDeviceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<DeviceDto>> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Devices.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<DeviceDto>.Fail("Device not found");
//                
//                var dto = _mapper.Map<DeviceDto>(entity);
//                return BaseResponse<DeviceDto>.Ok(dto);
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<DeviceDto>.Fail($"Error retrieving : {ex.Message}");
//            }
//        }
//    }
}
