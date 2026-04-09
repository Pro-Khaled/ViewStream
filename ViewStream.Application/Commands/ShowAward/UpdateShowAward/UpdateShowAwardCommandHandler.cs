using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.ShowAward.UpdateShowAward
{
//    public class UpdateShowAwardCommandHandler : IRequestHandler<UpdateShowAwardCommand, BaseResponse<ShowAwardDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateShowAwardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ShowAwardDto>> Handle(UpdateShowAwardCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.ShowAwards.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ShowAwardDto>.Fail("ShowAward not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.ShowAwards.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ShowAwardDto>(entity);
//                // return BaseResponse<ShowAwardDto>.Ok(dto, "ShowAward updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ShowAwardDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
