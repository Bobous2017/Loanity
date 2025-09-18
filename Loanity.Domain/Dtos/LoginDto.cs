using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos
{
    public class LoginDto
    {
        public string Username { get; set; } = null;
        public string Password { get; set; } = null!;
        public string? RfidChip { get; set; } // Only needed for Admin
    }

}
