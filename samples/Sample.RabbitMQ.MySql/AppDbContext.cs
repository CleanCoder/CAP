using Microsoft.EntityFrameworkCore;

namespace Sample.RabbitMQ.MySql
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;Database=testcap;UserId=root;Password=p@ssw0rd;");
        }
    }
}
