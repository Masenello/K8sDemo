using K8sBackendShared.Entities;
using Microsoft.EntityFrameworkCore;

namespace K8sBackendShared.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<TestData> Data {get;set;}
        public DbSet<TestJob> Jobs {get;set;}
        public DbSet<AppUser> Users { get; set; }
        
    }
}