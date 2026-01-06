using AutoMapper;
using Core.DTOs.Salary;
using Core.Models;

namespace Infrastructure.MappingProfiles
{
    public class SalaryPeriodProfile : Profile
    {
        public SalaryPeriodProfile()
        {
            CreateMap<SalaryPeriod, SalaryPeriodDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.LastName}"))
                .ForMember(dest => dest.MonthLabel, opt => opt.MapFrom(src => src.MonthLabel))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.Month))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary))
                .ForMember(dest => dest.Bonus, opt => opt.MapFrom(src => src.Bonus))
                .ForMember(dest => dest.Deductions, opt => opt.MapFrom(src => src.Deductions))
                .ForMember(dest => dest.DueAmount, opt => opt.MapFrom(src => src.DueAmount))
                .ForMember(dest => dest.TotalPaidAmount, opt => opt.MapFrom(src => src.TotalPaidAmount))
                .ForMember(dest => dest.RemainingAmount, opt => opt.MapFrom(src => src.RemainingAmount))
                .ForMember(dest => dest.SalaryPayments, opt => opt.MapFrom(src => src.SalaryPayments))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        }
    }
}
