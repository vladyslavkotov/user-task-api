using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserTaskApi.Dtos;
using UserTaskApi.Models;

[Route("api/[controller]")]
[ApiController]
public class UserTasksController : ControllerBase
{
   private readonly ApiDbContext _context;

   public UserTasksController(ApiDbContext context)
   {
      _context = context;
   }

   [HttpGet("{status}/{description}")]
   public async Task<ActionResult<IEnumerable<UserTaskDto>>> GetUserTask(string status, string description)
   {
      if (!Enum.TryParse<UserTaskState>(status.ToLower(), true, out var taskState))
      {
         return BadRequest("Invalid status.");
      }

      var userTasks = await _context.UserTasks.Where(x => x.State == taskState && x.Description.ToLower() == description.ToLower()).Select(x=>new UserTaskDto(x)).ToListAsync();
      if (userTasks.Count == 0)
      {  
         return NotFound();
      }

      return userTasks;
   }

   [HttpPost("{description}")]
   public async Task<ActionResult<UserTaskDto>> CreateUserTask(string description)
   {
      if (string.IsNullOrEmpty(description))
      {
         return BadRequest("UserTask description cannot be empty.");
      }
      var availableUsers = await _context.Users.Include(user=>user.UserTasks).Where(user => user.IsAvailable).ToListAsync();
      var newTask = new UserTask(description);
      if (availableUsers.Count != 0)
      {
         var randomUser = availableUsers[new Random().Next(0, availableUsers.Count)];
         if (randomUser is not null)
         {
            randomUser.AddUserTask(newTask);
            newTask.AssignUserOnCreate(randomUser);
         }
      }
      _context.UserTasks.Add(newTask);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(CreateUserTask), new UserTaskDto(newTask));
   }
}