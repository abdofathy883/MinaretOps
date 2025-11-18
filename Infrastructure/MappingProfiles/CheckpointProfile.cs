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

            CreateMap<ClientServiceCheckpoint, ClientServiceCheckpointDTO>()
                .ForMember(dest => dest.ServiceCheckpointName,
                    opt => opt.MapFrom(src => src.ServiceCheckpoint != null ? src.ServiceCheckpoint.Name : string.Empty))
                .ForMember(dest => dest.ServiceCheckpointDescription,
                    opt => opt.MapFrom(src => src.ServiceCheckpoint != null ? src.ServiceCheckpoint.Description : null))
                .ForMember(dest => dest.ServiceCheckpointOrder,
                    opt => opt.MapFrom(src => src.ServiceCheckpoint != null ? src.ServiceCheckpoint.Order : 0))
                .ForMember(dest => dest.CompletedByEmployeeName,
                    opt => opt.MapFrom(src => src.CompletedByEmployee != null
                        ? $"{src.CompletedByEmployee.FirstName} {src.CompletedByEmployee.LastName}"
                        : null));
        }
    }
}