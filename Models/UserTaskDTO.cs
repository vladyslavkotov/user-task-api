namespace testTask.Models;

public class UserTaskDTO
{
   public string Description { get; set; }

   public string User { get; set; }

   public string State { get; set; }

   public UserTaskDTO(UserTask task)
   {
      Description = task.Description;
      User = task.UserName ?? "null";
      State = task.State.ToString().ToLower();
   }
}