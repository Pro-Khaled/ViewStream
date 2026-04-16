using AutoMapper;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using MappingProfile = AutoMapper.Profile;
//using ViewStream.Application.DTOs;

namespace ViewStream.Application.Mappings
{
    public class DeviceMappingProfile : MappingProfile
    {
          public DeviceMappingProfile()
        {
            CreateMap<Device, DeviceDto>();
            CreateMap<Device, DeviceListItemDto>();
            CreateMap<CreateDeviceDto, Device>();
            CreateMap<UpdateDeviceDto, Device>();
        }
    }
}
