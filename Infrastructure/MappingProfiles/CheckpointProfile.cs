using AutoMapper;
using Core.DTOs.Checkpoints;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class CheckpointProfile : Profile
    {
        public CheckpointProfile()
        {
            CreateMap<ServiceCheckpoint, ServiceCheckpointDTO>();
            CreateMap<CreateServiceCheckpointDTO, ServiceCheckpoint>();
        }
    }
}