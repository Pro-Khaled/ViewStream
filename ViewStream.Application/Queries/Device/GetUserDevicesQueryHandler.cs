using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.Device
{
    public class GetUserDevicesQueryHandler : IRequestHandler<GetUserDevicesQuery, List<DeviceListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetUserDevicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DeviceListItemDto>> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
        {
            var devices = await _unitOfWork.Devices.FindAsync(
                d => d.UserId == request.UserId,
                asNoTracking: true, cancellationToken: cancellationToken);

            return _mapper.Map<List<DeviceListItemDto>>(devices.OrderByDescending(d => d.LastActive));
        }
    }
}
