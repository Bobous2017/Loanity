using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos
{
    public record UserDto(
         int Id,
         string FirstName,
         string LastName,
         string Email,
         string? Phone,
         int RoleId,
         string? RoleName
     );
}
