using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.LoginSession.UpdateLoginSession
{
//    public class UpdateLoginSessionCommandHandler : IRequestHandler<UpdateLoginSessionCommand, BaseResponse<LoginSessionDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdateLoginSessionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<LoginSessionDto>> Handle(UpdateLoginSessionCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.LoginSessions.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<LoginSessionDto>.Fail("LoginSession not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.LoginSessions.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<LoginSessionDto>(entity);
//                // return BaseResponse<LoginSessionDto>.Ok(dto, "LoginSession updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<LoginSessionDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
