using UserTaskApi.Models;

namespace UserTaskApi.Dtos;

public class UserDto
{
    public string Name { get; set; }

    public string Tasks { get; set; }

    public UserDto(User user)
    {
        Name = user.Name;
        Tasks = user.UserTasks.Count != 0 ? string.Join(", ", user.UserTasks.Select(x => $"Description: {x.Description}; State: {x.State.ToString().ToLower()}")) : "null";
    }
}
