using K8sBackendShared.Entities;
using Microsoft.EntityFrameworkCore;

namespace K8sBackendShared.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<TestDataEntity> Data {get;set;}
        public DbSet<TestJobEntity> Jobs {get;set;}
        public DbSet<AppUserEntity> Users { get; set; }
        
    }
}