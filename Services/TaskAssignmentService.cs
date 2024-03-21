using Microsoft.EntityFrameworkCore;
using testTask.Models;

namespace testTask.Services;

public class TaskAssignmentService : BackgroundService
{
   private readonly IServiceScopeFactory _serviceScopeFactory;
   private readonly TimeSpan timeInterval = TimeSpan.FromMinutes(2);

   public TaskAssignmentService(IServiceScopeFactory serviceScopeFactory)
   {
      _serviceScopeFactory = serviceScopeFactory;
   }

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      while (!stoppingToken.IsCancellationRequested)
      {
         await ReassignTasksToNewUsers();
         await Task.Delay(timeInterval, stoppingToken);
      }
   }

   private async Task ReassignTasksToNewUsers()
   {
      using var scope = _serviceScopeFactory.CreateScope();
      var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

      var tasksToReassign = await dbContext.UserTasks.Where(task => task.State != UserTaskState.Completed).ToListAsync();
      foreach (var task in tasksToReassign)
      {
         User? currentUser = null;
         if (task.UserId is not null)
         {
            currentUser= await dbContext.Users.Include(user=>user.UserTasks).SingleOrDefaultAsync(user=>user.Id == task.UserId);
            currentUser?.RemoveUserTask(task);
         }
         var newAvailableUser = await GetRandomNewUser(dbContext, currentUser);
         task.ResetUser();
         if (newAvailableUser != null)
         {
            if (task.TryReassignUser(newAvailableUser))
            {
               newAvailableUser.AddUserTask(task);
            }
            await dbContext.SaveChangesAsync();
         }
      }
   }

   private async Task<User?> GetRandomNewUser(ApiDbContext dbContext, User? currentUser)
   {
      var availableUsers = await dbContext.Users.Include(user=>user.UserTasks).Where(user => user.IsAvailable && (currentUser == null || user.Id != currentUser.Id)).ToListAsync();
      if (availableUsers.Count != 0)
      {
         return availableUsers[new Random().Next(0, availableUsers.Count)];
      }
      return null;
   }
}