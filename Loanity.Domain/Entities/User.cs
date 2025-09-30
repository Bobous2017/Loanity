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

        [System.Text.Json.Serialization.JsonIgnore]
        public Role? Role { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? PassWord { get; set; }
        public string? RfidChip { get; set; }
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        // NEW: explicit inverse navigation for Loan -> User
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
