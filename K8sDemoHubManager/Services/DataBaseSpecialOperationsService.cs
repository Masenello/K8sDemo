using System;
using System.Threading.Tasks;
using K8sBackendShared.Data;

namespace K8sDemoHubManager.Services
{
    public class DataBaseSpecialOperationsService
    {
        private readonly DataContext _context;
        public DataBaseSpecialOperationsService(DataContext context)
        {
            _context = context;
        }

        public async Task CleanConnectionsTable()
        {
            try
            {
                foreach(var connection in _context.ConnectedApps)
                    {
                        Console.WriteLine($"Sono nel for each");
                        _context.ConnectedApps.Remove(connection);
                    }
                    await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
            }
    
        
        }
    }
}