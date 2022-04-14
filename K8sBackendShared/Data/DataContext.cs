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
        public DbSet<JobEntity> Jobs {get;set;}
        public DbSet<AppUserEntity> Users { get; set; }
        public DbSet<ConnectedAppEntity> ConnectedApps { get; set; }
        
    }
}