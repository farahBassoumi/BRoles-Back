using BRoles.Models;
using Microsoft.EntityFrameworkCore;

namespace BRoles.Data
{
    public class OperationsDBContext:DbContext
    {

        public OperationsDBContext(DbContextOptions<OperationsDBContext> options) : base(options)
        {
        }
        public DbSet<Operations> UserOperations { get; set; }//the database table 
    }
}
