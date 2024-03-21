using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace testTask.Models;

[Index(nameof(Name),IsUnique =true)]
public class User
{
   private readonly static int MaxUserTaskCount = 5;

   public int Id { get; private set; }

   [MaxLength(50)]
   public string Name { get; }

   public bool IsAvailable { get; private set; } = true;

   public List<UserTask> UserTasks { get; } = new ();

   public User(string name)
   {
      Name = name;
   }

   public void AddUserTask(UserTask userTask)
   {
      UserTasks.Add(userTask);
      if (UserTasks.Count == MaxUserTaskCount)
      {
         IsAvailable = false;
      }
   }

   public void RemoveUserTask(UserTask userTask)
   {
      UserTasks.Remove(userTask);
      IsAvailable = true;
   }
}