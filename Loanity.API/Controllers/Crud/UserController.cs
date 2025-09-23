using Loanity.API.Controllers.Common;
using Loanity.Domain.AuthHelper;
using Loanity.Domain.Dtos;
using Loanity.Domain.Entities;
using Loanity.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Loanity.API.Controllers.Crud
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : CrudControllerAPI<User>
    {
        public UsersController(LoanityDbContext db) : base(db) { }

        // Override GET ALL to return UserDto with Role.Name
        [HttpGet]
        public override async Task<IActionResult> GetAll()
        {
            var users = await _db.Users.Include(u => u.Role).ToListAsync();

            var userDtos = users.Select(u => new UserDto(
                u.Id,
                u.FirstName,
                u.LastName,
                u.UserName,
                u.PassWord,
                u.RfidChip,
                u.Email,
                u.Phone,
                u.RoleId,
                u.Role?.Name,
                null // ConfirmAdminPassword is not returned
            )).ToList();

            return Ok(userDtos);
        }
       

        [HttpPost]  //  Når vi skaber en bruger,  så  hasher vi  password med det samme
        public override async Task<IActionResult> Create([FromBody] User user)
        {
            if (!string.IsNullOrWhiteSpace(user.PassWord))
            {
                user.PassWord = PasswordHelper.Hash(user.PassWord);
            }

            return await base.Create(user);
        }


        [Authorize]
        [HttpPut("{id}/dto")] //  Vi bruger Token,  for at ved  hvem vil  ændre password til andre bruger, Authorize, Verifying Hashing, 
        public async Task<IActionResult> UpdateDto(int id, [FromBody] UserDto dto)
        {
            var currentUserName = User.Identity?.Name;  // now comes from JWT
            Console.WriteLine("[DEBUG] User.Identity?.Name: " + currentUserName);

            var currentUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == currentUserName);

            if (currentUser == null || !PasswordHelper.Verify(dto.ConfirmAdminPassword, currentUser.PassWord))
            {
                return Unauthorized("You must confirm your password to update another account.");
            }

            var existingUser = await _db.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.FirstName = dto.FirstName;
            existingUser.LastName = dto.LastName;
            existingUser.UserName = dto.UserName;
            existingUser.Email = dto.Email;
            existingUser.Phone = dto.Phone;
            existingUser.RoleId = dto.RoleId;
            existingUser.RfidChip = dto.RfidChip;

            if (!string.IsNullOrWhiteSpace(dto.PassWord))
                existingUser.PassWord = PasswordHelper.Hash(dto.PassWord);

            await _db.SaveChangesAsync();
            return NoContent();
        }


        // Optional: keep default GetById if DTO isn't needed
        // Otherwise, override it the same way as above.  Now

    }
}
