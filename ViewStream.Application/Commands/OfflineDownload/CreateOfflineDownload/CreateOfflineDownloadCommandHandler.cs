using MediatR;
using AutoMapper;
using ViewStream.Application.Common;
//using ViewStream.Application.DTOs;
using ViewStream.Domain.Entities;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Commands.OfflineDownload.CreateOfflineDownload
{
  //  public class CreateOfflineDownloadCommandHandler : IRequestHandler<CreateOfflineDownloadCommand, BaseResponse<OfflineDownloadDto>>
  //  {
  //      private readonly IUnitOfWork _unitOfWork;
  //      private readonly IMapper _mapper;

  //      public CreateOfflineDownloadCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  //      {
  //          _unitOfWork = unitOfWork;
  //          _mapper = mapper;
  //      }

  //      public async Task<BaseResponse<OfflineDownloadDto>> Handle(CreateOfflineDownloadCommand request, CancellationToken cancellationToken)
  //      {
  //          try
  //          {
  //              // TODO: Map request to entity
  //              // var entity = _mapper.Map<OfflineDownload>(request);
  //              
  //              // await _unitOfWork.OfflineDownloads.AddAsync(entity);
  //              // await _unitOfWork.SaveChangesAsync();
  //              
  //              // var dto = _mapper.Map<OfflineDownloadDto>(entity);
  //              // return BaseResponse<OfflineDownloadDto>.Ok(dto, "OfflineDownload created successfully");
  //              
  //              throw new NotImplementedException();
  //          }
  //          catch (Exception ex)
  //          {
  //              return BaseResponse<OfflineDownloadDto>.Fail($"Error creating : {ex.Message}");
  //          }
  //      }
  //  }
}
