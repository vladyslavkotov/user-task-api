using Microsoft.EntityFrameworkCore;

namespace testTask.Models;

[Index(nameof(Description), nameof(State))]
public class UserTask
{
   private static readonly int MaxReassignCount = 3;

   public int Id { get; private set; }

   public string Description { get; }

   public int? UserId { get; private set; }

   public string? UserName { get; private set; }

   public UserTaskState State { get; private set; } = UserTaskState.Waiting;

   public int AssignCount { get; private set; }

   public UserTask(string description)
   {
      Description = description;
   }

   public bool TryReassignUser(User newUser)
   {
      AssignCount++;
      if (AssignCount > MaxReassignCount)
      {
         State = UserTaskState.Completed;
         UserId = null;
         UserName = null;
         return false;
      }
      else
      {
         State = UserTaskState.InProgress;
         UserId = newUser.Id;
         UserName = newUser.Name;
         return true;
      }
   }

   public void AssignUserOnCreate(User newUser)
   {
      State = UserTaskState.InProgress;
      UserId = newUser.Id;
      UserName = newUser.Name;
   }

   public void ResetUser()
   {
      State = UserTaskState.Waiting;
      UserId = null;
      UserName = null;
   }
}