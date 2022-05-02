using K8sBackendShared.Data;

namespace K8sBackendShared.Repository.JobRepository
{
    public class JobUnitOfWork:IJobUnitOfWork
    {
        private readonly DataContext _context;

        public IJobRepository Jobs { get; private set; }

        public JobUnitOfWork(DataContext context)
        {
            _context = context;
            Jobs = new JobRepository(_context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}