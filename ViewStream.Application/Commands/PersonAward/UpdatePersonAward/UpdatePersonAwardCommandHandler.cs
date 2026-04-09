using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.PersonAward.UpdatePersonAward
{
//    public class UpdatePersonAwardCommandHandler : IRequestHandler<UpdatePersonAwardCommand, BaseResponse<PersonAwardDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePersonAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PersonAwardDto>> Handle(UpdatePersonAwardCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.PersonAwards.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PersonAwardDto>.Fail("PersonAward not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.PersonAwards.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PersonAwardDto>(entity);
//                // return BaseResponse<PersonAwardDto>.Ok(dto, "PersonAward updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PersonAwardDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
