using AutoMapper;
using Core.DTOs.Contract;
using Core.DTOs.Salary;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Contracts;

namespace Infrastructure.Services.Payroll
{
    public class PayrollService : IPayrollService
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<PayrollService> logger;
        public PayrollService(
            MinaretOpsDbContext _dbContext,
            IMapper _mapper,
            ILogger<PayrollService> _logger)
        {
            dbContext = _dbContext;
            mapper = _mapper;
            logger = _logger;
        }

        public async Task<SalaryPeriodDTO> CreateSalaryPeriodAsync(CreateSalaryPeriodDTO createSalaryPeriodDTO)
        {
            // Validate employee exists
            var employee = await dbContext.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == createSalaryPeriodDTO.EmployeeId);

            if (employee == null)
                throw new InvalidObjectException("الموظف غير موجود");

            // Determine month and year from current date or use provided date
            var now = DateTime.UtcNow;
            var month = createSalaryPeriodDTO.CreatedAt.Month;
            var year = createSalaryPeriodDTO.CreatedAt.Year;
            var monthLabel = $"{year}-{month:D2}";

            // Check if period already exists
            var existingPeriod = await dbContext.SalaryPeriods
                .FirstOrDefaultAsync(sp => sp.EmployeeId == createSalaryPeriodDTO.EmployeeId
                    && sp.Year == year
                    && sp.Month == month);

            if (existingPeriod != null)
                throw new AlreadyExistObjectException($"فترة الراتب لهذا الشهر موجودة بالفعل");

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var salaryPeriod = new SalaryPeriod
                {
                    EmployeeId = createSalaryPeriodDTO.EmployeeId,
                    MonthLabel = monthLabel,
                    Month = month,
                    Year = year,
                    Salary = createSalaryPeriodDTO.Salary,
                    Bonus = createSalaryPeriodDTO.Bonus,
                    Deductions = createSalaryPeriodDTO.Deductions,
                    Notes = createSalaryPeriodDTO.Notes,
                    CreatedAt = createSalaryPeriodDTO.CreatedAt
                };

                dbContext.SalaryPeriods.Add(salaryPeriod);
                await dbContext.SaveChangesAsync();

                // Create salary payments if any
                if (createSalaryPeriodDTO.SalaryPayments != null && createSalaryPeriodDTO.SalaryPayments.Any())
                {
                    foreach (var paymentDto in createSalaryPeriodDTO.SalaryPayments)
                    {
                        var payment = new SalaryPayment
                        {
                            EmployeeId = paymentDto.EmployeeId,
                            SalaryPeriodId = salaryPeriod.Id,
                            Amount = paymentDto.Amount,
                            Notes = paymentDto.Notes,
                            CreatedAt = DateTime.UtcNow
                        };
                        dbContext.SalaryPayments.Add(payment);
                    }
                    await dbContext.SaveChangesAsync();
                }

                await dbTransaction.CommitAsync();

                // Reload with navigation properties
                var createdPeriod = await dbContext.SalaryPeriods
                    .Include(sp => sp.Employee)
                    .Include(sp => sp.SalaryPayments)
                    .FirstOrDefaultAsync(sp => sp.Id == salaryPeriod.Id);

                return mapper.Map<SalaryPeriodDTO>(createdPeriod);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                logger.LogError(ex, "Error creating salary period");
                throw new InvalidObjectException($"خطأ في إنشاء فترة الراتب: {ex.Message}");
            }
        }
        public async Task<SalaryPaymentDTO> RecordSalaryPaymentAsync(CreateSalaryPaymentDTO createSalaryPaymentDTO)
        {
            // Validate employee exists
            var employee = await dbContext.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == createSalaryPaymentDTO.EmployeeId);

            if (employee == null)
                throw new InvalidObjectException("الموظف غير موجود");

            // If SalaryPeriodId is provided, validate it exists and belongs to the employee
            if (createSalaryPaymentDTO.SalaryPeriodId.HasValue)
            {
                var period = await dbContext.SalaryPeriods
                    .FirstOrDefaultAsync(sp => sp.Id == createSalaryPaymentDTO.SalaryPeriodId.Value
                        && sp.EmployeeId == createSalaryPaymentDTO.EmployeeId);

                if (period == null)
                    throw new InvalidObjectException("فترة الراتب غير موجودة أو لا تنتمي لهذا الموظف");
            }

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var payment = new SalaryPayment
                {
                    EmployeeId = createSalaryPaymentDTO.EmployeeId,
                    SalaryPeriodId = createSalaryPaymentDTO.SalaryPeriodId,
                    Amount = createSalaryPaymentDTO.Amount,
                    Notes = createSalaryPaymentDTO.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                dbContext.SalaryPayments.Add(payment);
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                // Reload with navigation properties
                var createdPayment = await dbContext.SalaryPayments
                    .Include(sp => sp.Employee)
                    .Include(sp => sp.SalaryPeriod)
                    .FirstOrDefaultAsync(sp => sp.Id == payment.Id);

                return mapper.Map<SalaryPaymentDTO>(createdPayment);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                logger.LogError(ex, "Error recording salary payment");
                throw new InvalidObjectException($"خطأ في تسجيل الدفع: {ex.Message}");
            }
        }
        public async Task<SalaryPeriodDTO?> GetSalaryPeriodAsync(string employeeId, int month, int year)
        {
            var period = await dbContext.SalaryPeriods
                .Include(sp => sp.Employee)
                .Include(sp => sp.SalaryPayments)
                .FirstOrDefaultAsync(sp => sp.EmployeeId == employeeId
                    && sp.Month == month
                    && sp.Year == year);

            return period == null ? null : mapper.Map<SalaryPeriodDTO>(period);
        }
        public async Task<List<SalaryPeriodDTO>> GetSalaryPeriodsAsync(string employeeId)
        {
            var periods = await dbContext.SalaryPeriods
                .Include(sp => sp.Employee)
                .Include(sp => sp.SalaryPayments)
                .Where(sp => sp.EmployeeId == employeeId)
                .OrderByDescending(sp => sp.Year)
                .ThenByDescending(sp => sp.Month)
                .ToListAsync();

            return mapper.Map<List<SalaryPeriodDTO>>(periods);
        }

        public async Task<List<SalaryPeriodDTO>> GetAllSalaryPeriodsAsync()
        {
            var periods = await dbContext.SalaryPeriods
                .Include(sp => sp.Employee)
                .Include(sp => sp.SalaryPayments)
                .OrderByDescending(sp => sp.Year)
                .ThenByDescending(sp => sp.Month)
                .ThenBy(sp => sp.Employee.FirstName)
                .ToListAsync();

            return mapper.Map<List<SalaryPeriodDTO>>(periods);
        }
        public async Task<SalaryPeriodDTO?> GetSalaryPeriodByIdAsync(int periodId)
        {
            var period = await dbContext.SalaryPeriods
                .Include(sp => sp.Employee)
                .Include(sp => sp.SalaryPayments)
                .FirstOrDefaultAsync(sp => sp.Id == periodId);

            return period == null ? null : mapper.Map<SalaryPeriodDTO>(period);
        }

        public async Task<List<SalaryPaymentDTO>> GetSalaryPaymentsAsync(string employeeId)
        {
            var payments = await dbContext.SalaryPayments
                .Include(sp => sp.Employee)
                .Include(sp => sp.SalaryPeriod)
                .Where(sp => sp.EmployeeId == employeeId)
                .OrderByDescending(sp => sp.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<SalaryPaymentDTO>>(payments);
        }

        public async Task<SalaryPeriodDTO> UpdateSalaryPeriod(UpdateSalaryPeriodDTO updateSalary)
        {
            var period = await dbContext.SalaryPeriods
                .FirstOrDefaultAsync(sp => sp.Id == updateSalary.Id)
                ?? throw new KeyNotFoundException();

            if (period.Deductions != updateSalary.Deductions
                && period.Deductions != 0)
                period.Deductions = period.Deductions;

            if (period.Bonus != updateSalary.Bonus
                && period.Bonus != 0)
                period.Bonus = period.Bonus;

            if (period.Notes != updateSalary.Notes
                && !string.IsNullOrWhiteSpace(updateSalary.Notes))
                period.Notes = period.Notes;

            dbContext.SalaryPeriods.Update(period);
            await dbContext.SaveChangesAsync();
            return mapper.Map<SalaryPeriodDTO>(period);
        }
    }
}
