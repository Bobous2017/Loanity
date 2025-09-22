using Loanity.API.Controllers.Common;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : CrudControllerAPI<Reservation>
    {
        public ReservationController(LoanityDbContext db) : base(db) { }
    }
}
