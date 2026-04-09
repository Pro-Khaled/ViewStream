using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Country.UpdateCountry
{
//    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, BaseResponse<CountryDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateCountryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Countrys.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<CountryDto>.Fail("Country not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Countrys.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<CountryDto>(entity);
//                // return BaseResponse<CountryDto>.Ok(dto, "Country updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<CountryDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
