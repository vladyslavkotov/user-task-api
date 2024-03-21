using UserTaskApi.Models;

namespace UserTaskApi.Dtos;

public class UserTaskDto
{
    public string Description { get; set; }

    public string User { get; set; }

    public string State { get; set; }

    public UserTaskDto(UserTask task)
    {
        Description = task.Description;
        User = task.UserName ?? "null";
        State = task.State.ToString().ToLower();
    }
}