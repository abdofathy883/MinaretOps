using Core.DTOs.KPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IKPIService
    {
        Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto);
    }
}
