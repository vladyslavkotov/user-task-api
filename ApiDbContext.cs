using Microsoft.EntityFrameworkCore;

namespace testTask.Models;

public class ApiDbContext : DbContext
{
   public DbSet<UserTask> UserTasks { get; set; }
   public DbSet<User> Users { get; set; }   

   public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
   {
      this.ChangeTracker.LazyLoadingEnabled = false;
   }

   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
   {
      optionsBuilder.UseSqlite("Data Source=database.mdf");
   }
}
