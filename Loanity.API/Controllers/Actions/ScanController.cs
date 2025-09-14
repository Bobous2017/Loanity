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
        //Loan logic( Injection ) business rules ----------------3
        private readonly ILoanService _loanService;
        public ScanController(ILoanService loanService) => _loanService = loanService;

        // ------------ Scan (Loan or Return) ------------
        [HttpPost]
        public async Task<IActionResult> Post(ScanRequestDto req)
        {
            switch (req.Action.ToLower())
            {
                case "loan":
                    var loan = await _loanService.CreateLoanFromScanAsync( //  loaning
                        req.UserId, req.QrCode, req.DueAt ?? DateTime.UtcNow.AddDays(7));

                    return Ok(new ScanResultDto
                    {
                        LoanId = loan.Id,
                        Message = "Loan successfully created"
                    });

                case "return":
                    var returnedLoan = await _loanService.ReturnByScanAsync(req.UserId, req.QrCode); // /returning 

                    if (returnedLoan is null)
                        return NotFound(new { Message = "No active loan found to return." });

                    return Ok(new { Message = "Device successfully returned" });

                default:
                    return BadRequest("Unknown action: use 'loan' or 'return'");
            }
        }
    }
}
