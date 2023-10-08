using BRoles.Models;
using Microsoft.EntityFrameworkCore;


namespace BRoles.Data
{
    public class FullStackDBContext : DbContext
    {
        public FullStackDBContext (DbContextOptions<FullStackDBContext> options) : base(options)
        {
        }
        public DbSet<UserDto> users { get; set; }//the database table 
    }
}
