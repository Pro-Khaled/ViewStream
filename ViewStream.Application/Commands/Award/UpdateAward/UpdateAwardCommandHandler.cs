using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Award.UpdateAward
{
//    public class UpdateAwardCommandHandler : IRequestHandler<UpdateAwardCommand, BaseResponse<AwardDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<AwardDto>> Handle(UpdateAwardCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Awards.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<AwardDto>.Fail("Award not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Awards.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<AwardDto>(entity);
//                // return BaseResponse<AwardDto>.Ok(dto, "Award updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<AwardDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
