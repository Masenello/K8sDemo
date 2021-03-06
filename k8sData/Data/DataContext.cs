
using K8sCore.Entities;
using K8sCore.Entities.Ef;
using Microsoft.EntityFrameworkCore;

namespace K8sData.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<TestDataEntity> Data { get; set; }
        public DbSet<ConnectedAppEntity> ConnectedApps { get; set; }
        public DbSet<AppUserEntity> Users { get; set; }
        public DbSet<AppRoleEntity> Roles { get; set; }
        public DbSet<AppUserRoleMapperEntity> UserRolesMapper { get; set; }
    }
}