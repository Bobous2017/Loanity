using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public int RoleId { get; set; }

       // [ValidateNever]
        [System.Text.Json.Serialization.JsonIgnore]
        public Role? Role { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        // address fields omitted for brevity
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>(); // ← ADD THIS

    }
}
