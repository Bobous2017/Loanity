using Loanity.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.AuthHelper
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }

}
