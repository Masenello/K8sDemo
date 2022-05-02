using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using K8sCore.Entities;
using K8sCore.Enums;
using K8sCore.Interfaces.JobRepository;
using K8sCore.Messages;
using K8sData;
using K8sData.Data;
using Microsoft.EntityFrameworkCore;

namespace K8sBackendShared.Repository.JobRepository
{
    public class JobRepository : GenericRepository<JobEntity>, IJobRepository
    {
        public JobRepository(DataContext context): base(context)
        { 

        }


        public async Task<IEnumerable<JobEntity>> GetJobsInStatusAsync(JobStatus targetStatus)
        {
            return await _context.Jobs.Where(x=>x.Status == JobStatus.created).Include(u=>u.User).ToListAsync();
        }

        public async Task<JobEntity> GetJobWithIdAsync(int Id)
        {
            return await _context.Jobs.Include(u=>u.User).FirstOrDefaultAsync(x=>x.Id == Id);
        }

    }

}