using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonalizedRow.UpdatePersonalizedRow
{
//    public class UpdatePersonalizedRowCommandHandler : IRequestHandler<UpdatePersonalizedRowCommand, BaseResponse<PersonalizedRowDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePersonalizedRowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PersonalizedRowDto>> Handle(UpdatePersonalizedRowCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PersonalizedRows.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PersonalizedRowDto>.Fail("PersonalizedRow not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.PersonalizedRows.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PersonalizedRowDto>(entity);
//                // return BaseResponse<PersonalizedRowDto>.Ok(dto, "PersonalizedRow updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PersonalizedRowDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
