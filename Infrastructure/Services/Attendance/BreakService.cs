using AutoMapper;
using Core.DTOs.AttendanceBreaks;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Attendance
{
    public class BreakService : IBreakService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;

        public BreakService(MinaretOpsDbContext context, TaskHelperService helperService, IMapper _mapper)
        {
            this.context = context;
            this.helperService = helperService;
            mapper = _mapper;
        }

        public async Task<BreakDTO> EndBreakAsync(Start_EndBreakDTO breakDTO)
        {
            var user = await helperService.GetUserOrThrow(breakDTO.EmployeeId);

            var egyptToday = TimeZoneHelper.GetEgyptToday();

            // Get today's attendance record
            var attendanceRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == breakDTO.EmployeeId && r.WorkDate == egyptToday);

            if (attendanceRecord == null)
                throw new InvalidObjectException("لا يوجد سجل حضور لهذا اليوم");

            // Find active break
            var activeBreak = await context.BreakPeriods
                .FirstOrDefaultAsync(b => b.AttendanceRecordId == attendanceRecord.Id && b.EndTime == null);

            if (activeBreak == null)
                throw new InvalidObjectException("لا توجد استراحة نشطة");

            activeBreak.EndTime = DateTime.UtcNow;
            context.Update(activeBreak);
            await context.SaveChangesAsync();

            return mapper.Map<BreakDTO>(activeBreak);
        }

        public async Task<BreakDTO?> GetActiveBreakAsync(string employeeId)
        {
            var egyptToday = TimeZoneHelper.GetEgyptToday();
            var attendanceRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == employeeId && r.WorkDate == egyptToday);

            if (attendanceRecord == null)
                return null;

            var activeBreak = await context.BreakPeriods
                .FirstOrDefaultAsync(b => b.AttendanceRecordId == attendanceRecord.Id && b.EndTime == null);

            return activeBreak != null ? mapper.Map<BreakDTO>(activeBreak) : null;
        }

        public async Task<BreakDTO> StartBreakAsync(Start_EndBreakDTO breakDTO)
        {
            var user = await helperService.GetUserOrThrow(breakDTO.EmployeeId);

            var egyptToday = TimeZoneHelper.GetEgyptToday();

            // Get today's attendance record
            var attendanceRecord = await context.AttendanceRecords
                .FirstOrDefaultAsync(r => r.EmployeeId == breakDTO.EmployeeId && r.WorkDate == egyptToday);

            if (attendanceRecord == null)
                throw new InvalidObjectException("يجب تسجيل الحضور أولاً قبل بدء الاستراحة");

            if (attendanceRecord.ClockOut.HasValue)
                throw new InvalidObjectException("لا يمكن بدء استراحة بعد تسجيل الانصراف");

            // Check if there's already an active break
            var activeBreak = await context.BreakPeriods
                .FirstOrDefaultAsync(b => b.AttendanceRecordId == attendanceRecord.Id && b.EndTime == null);

            if (activeBreak != null)
                throw new InvalidObjectException("يوجد استراحة نشطة بالفعل");

            var breakPeriod = new BreakPeriod
            {
                AttendanceRecordId = attendanceRecord.Id,
                StartTime = DateTime.UtcNow
            };

            await context.BreakPeriods.AddAsync(breakPeriod);
            await context.SaveChangesAsync();

            return mapper.Map<BreakDTO>(breakPeriod);
        }
    }
}
