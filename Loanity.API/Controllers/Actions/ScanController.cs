using Loanity.Domain.Dtos;
using Loanity.Domain.Dtos.ScanType;
using Loanity.Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers.Actions
{
    [ApiController]
    [Route("api/scan")]
    public class ScanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public ScanController(ILoanService loanService) => _loanService = loanService;

        // ------------ Scan (Loan or Return) ------------
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ScanRequestDto req)
        {
            if (req is null)
                return BadRequest(new { Message = "Request body is required." });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var action = req.Action?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(action))
                return BadRequest(new { Message = "Action is required. Use 'loan' or 'return'." });

            switch (action)
            {
                case "loan":
                    try
                    {
                        if (req.DueAt is not null && req.DueAt.Value.Year > 2100)
                        {
                            return BadRequest(new { Message = "Due date is not realistic." });
                        }

                        var dueAt = req.DueAt ?? DateTime.UtcNow.AddDays(7);

                        var loan = await _loanService.CreateLoanFromScanAsync(req.UserId, req.QrCode, dueAt);

                        // Handle the case where the service signals failure by returning null
                        if (loan is null)
                            return NotFound(new { Message = "Unable to create loan for the specified QR code." });

                        return Ok(new ScanResultDto
                        {
                            LoanId = loan.Id,
                            Message = "Loan successfully created"
                        });
                    }
                    catch (ArgumentException ex)
                    {
                        // e.g., invalid QR code, invalid parameters
                        return BadRequest(new { Message = ex.Message });
                    }
                    catch (InvalidOperationException ex)
                    {
                        // e.g., device already loaned, business rule violations
                        return Conflict(new { Message = ex.Message });
                    }
                    catch (Exception)
                    {
                        // Unexpected error
                        return Problem("An unexpected error occurred while creating the loan.");
                    }

                case "return":
                    try
                    {
                        var returnedLoan = await _loanService.ReturnByScanAsync(req.UserId, req.QrCode);

                        if (returnedLoan is null)
                            return NotFound(new { Message = "No active loan found to return." });

                        return Ok(new { Message = "Device successfully returned" });
                    }
                    catch (ArgumentException ex)
                    {
                        return BadRequest(new { Message = ex.Message });
                    }
                    catch (InvalidOperationException ex)
                    {
                        return Conflict(new { Message = ex.Message });
                    }
                    catch (Exception)
                    {
                        return Problem("An unexpected error occurred while returning the device.");
                    }

                default:
                    return BadRequest(new { Message = "Unknown action: use 'loan' or 'return'." });
            }
        }
    }
}
