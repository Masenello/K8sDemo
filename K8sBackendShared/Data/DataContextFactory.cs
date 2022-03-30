using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace K8sBackendShared.Data
{
//Questa classe è necessaria per creare il context nella class library in DESIGN time (migrations)
public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer("server=host.docker.internal;initial catalog=TestDatabase;persist security info=True;user id=sa;password=Pass@Word1;MultipleActiveResultSets=True;App=EntityFramework",
                    sqlServerOptions => sqlServerOptions.CommandTimeout(180));

        return new DataContext(optionsBuilder.Options);
    }

    }
}