using Core.DTOs.AttendanceBreaks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IBreakService
    {
        Task<BreakDTO> StartBreakAsync(Start_EndBreakDTO breakDTO);
        Task<BreakDTO> EndBreakAsync(Start_EndBreakDTO breakDTO);
        Task<BreakDTO?> GetActiveBreakAsync(string employeeId);
    }
}
