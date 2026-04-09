using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Show.UpdateShow
{
//    public class UpdateShowCommandHandler : IRequestHandler<UpdateShowCommand, BaseResponse<ShowDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateShowCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<ShowDto>> Handle(UpdateShowCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Shows.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<ShowDto>.Fail("Show not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Shows.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<ShowDto>(entity);
//                // return BaseResponse<ShowDto>.Ok(dto, "Show updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<ShowDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
