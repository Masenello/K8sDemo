using K8sBackendShared.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace K8sData.Data
{
//Questa classe è necessaria per creare il context nella class library in DESIGN time (migrations)
public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer(NetworkSettings.DatabaseConnectionStringResolver(),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(180));
                    

        return new DataContext(optionsBuilder.Options);
    }

    }
}