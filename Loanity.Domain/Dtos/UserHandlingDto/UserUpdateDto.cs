using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loanity.Domain.Dtos.UserHandlingDto
{
    public record UserUpdateDto(
     int Id,
     string FirstName,
     string LastName,
     string UserName,
     string? PassWord,
     string? RfidChip,
     string Email,
     string? Phone,
     int RoleId,
     string? RoleName,
     string? ConfirmAdminPassword
    );

}
