using Loanity.Domain.Dtos;
using Loanity.Domain.Dtos.ScanType;
using Loanity.Domain.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers
{
    [ApiController]
    [Route("api/scan")]
    public class ScanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public ScanController(ILoanService loanService) => _loanService = loanService;


        [HttpPost]
        public async Task<IActionResult> Post(ScanRequestDto req)
        {
            return req.Action.ToLower() switch
            {
                "loan" => Ok(new ScanResultDto
                {
                    LoanId = (await _loanService.CreateLoanFromScanAsync(req.UserId, req.QrCode, req.DueAt ?? DateTime.UtcNow.AddDays(7))).Id,
                    Message = "Loan successfully created"
                }),
                "return" => Ok(new { Message = "Device successfully returned" }),
                _ => BadRequest("Unknown action: use 'loan' or 'return'")
            };
        }

    }
}
