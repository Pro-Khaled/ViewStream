using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewStream.Application.DTOs;
using ViewStream.Domain.Interfaces;

namespace ViewStream.Application.Queries.OfflineDownload
{
    public class GetProfileDownloadsQueryHandler : IRequestHandler<GetProfileDownloadsQuery, List<OfflineDownloadListItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProfileDownloadsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<OfflineDownloadListItemDto>> Handle(GetProfileDownloadsQuery request, CancellationToken cancellationToken)
        {
            var downloads = await _unitOfWork.OfflineDownloads.FindAsync(
                d => d.ProfileId == request.ProfileId,
                include: q => q.Include(d => d.Episode),
                asNoTracking: true, cancellationToken: cancellationToken);

            return _mapper.Map<List<OfflineDownloadListItemDto>>(downloads.OrderByDescending(d => d.DownloadedAt));
        }
    }
}
