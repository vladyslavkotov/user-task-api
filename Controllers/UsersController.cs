using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testTask.Models;

namespace testTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
   private readonly ApiDbContext _context;

   public UsersController(ApiDbContext context)
   {
      _context = context;
   }

   [HttpGet("{name}")]
   public async Task<ActionResult<UserDTO>> GetUser(string name)
   {
      var user = await _context.Users.Include(user=>user.UserTasks).SingleOrDefaultAsync(x => x.Name == name);

      if (user == null)
      {
         return NotFound();
      }
      return new UserDTO(user);
   }

   [HttpPost("{name}")]
   public async Task<ActionResult<UserDTO>> CreateUser(string name)
   {
      if (string.IsNullOrEmpty(name))
      {
         return BadRequest("User name cannot be empty.");
      }

      if (await _context.Users.AnyAsync(u => u.Name == name))
      {
         return Conflict("User with this name already exists.");
      }

      var user = new User(name);
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return CreatedAtAction(nameof(CreateUser), new UserDTO(user));
   }
}