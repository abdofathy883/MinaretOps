using AutoMapper;
using Core.DTOs.Portfolio;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services.MediaUploads;

namespace Infrastructure.Services.Portfolio
{
    public class PortfolioCategoryService : IPortfolioCategoryService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;
        private readonly MediaUploadService mediaUploadService;

        public PortfolioCategoryService(MinaretOpsDbContext context, IMapper mapper, MediaUploadService mediaUploadService)
        {
            this.context = context;
            this.mapper = mapper;
            this.mediaUploadService = mediaUploadService;
        }

        public Task<PortfolioCategoryDTO> Create(CreatePortfolioCategoryDTO createCategory)
        {
            throw new NotImplementedException();
        }

        public Task<PortfolioCategoryDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PortfolioCategoryDTO> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
