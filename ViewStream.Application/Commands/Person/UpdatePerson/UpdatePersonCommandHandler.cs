using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.Person.UpdatePerson
{
//    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, BaseResponse<PersonDto>>
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//
//        public UpdatePersonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
//        {
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//        }
//
//        public async Task<BaseResponse<PersonDto>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var entity = await _unitOfWork.Persons.GetByIdAsync(request.Id);
//                if (entity == null)
//                    return BaseResponse<PersonDto>.Fail("Person not found");
//                
//                // TODO: Update entity properties
//                // _mapper.Map(request, entity);
//                // _unitOfWork.Persons.Update(entity);
//                // await _unitOfWork.SaveChangesAsync();
//                
//                // var dto = _mapper.Map<PersonDto>(entity);
//                // return BaseResponse<PersonDto>.Ok(dto, "Person updated successfully");
//                
//                throw new NotImplementedException();
//            }
//            catch (Exception ex)
//            {
//                return BaseResponse<PersonDto>.Fail($"Error updating : {ex.Message}");
//            }
//        }
//    }
}
