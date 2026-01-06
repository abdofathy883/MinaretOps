using Core.DTOs.Salary;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService payrollService;

        public PayrollController(IPayrollService _payrollService)
        {
            payrollService = _payrollService;
        }

        [HttpPost("salary-period")]
        public async Task<IActionResult> CreateSalaryPeriodAsync(CreateSalaryPeriodDTO createSalaryPeriodDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await payrollService.CreateSalaryPeriodAsync(createSalaryPeriodDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("salary-payment")]
        public async Task<IActionResult> RecordSalaryPaymentAsync(CreateSalaryPaymentDTO createSalaryPaymentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await payrollService.RecordSalaryPaymentAsync(createSalaryPaymentDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("salary-periods/employee/{employeeId}")]
        public async Task<IActionResult> GetSalaryPeriodsByEmployeeAsync(string employeeId)
        {
            try
            {
                var periods = await payrollService.GetSalaryPeriodsAsync(employeeId);
                return Ok(periods);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("salary-periods")]
        public async Task<IActionResult> GetAllSalaryPeriodsAsync()
        {
            try
            {
                var periods = await payrollService.GetAllSalaryPeriodsAsync();
                return Ok(periods);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("salary-period/{periodId}")]
        public async Task<IActionResult> GetSalaryPeriodByIdAsync(int periodId)
        {
            if (periodId == 0)
                return BadRequest();

            try
            {
                var period = await payrollService.GetSalaryPeriodByIdAsync(periodId);
                if (period == null)
                    return NotFound();
                return Ok(period);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("salary-period/employee/{employeeId}/month/{month}/year/{year}")]
        public async Task<IActionResult> GetSalaryPeriodAsync(string employeeId, int month, int year)
        {
            try
            {
                var period = await payrollService.GetSalaryPeriodAsync(employeeId, month, year);
                if (period == null)
                    return NotFound();
                return Ok(period);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("salary-payments/employee/{employeeId}")]
        public async Task<IActionResult> GetSalaryPaymentsByEmployeeAsync(string employeeId)
        {
            try
            {
                var payments = await payrollService.GetSalaryPaymentsAsync(employeeId);
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
