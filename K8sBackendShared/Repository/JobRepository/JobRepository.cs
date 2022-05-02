using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using K8sBackendShared.Data;
using K8sBackendShared.Entities;
using K8sBackendShared.Enums;
using Microsoft.EntityFrameworkCore;

namespace K8sBackendShared.Repository.JobRepository
{
    public class JobRepository : GenericRepository<JobEntity>, IJobRepository
    {
        public JobRepository(DataContext context): base(context)
        { 

        }

        public IEnumerable<JobEntity> GetJobsInStatus(JobStatus targetStatus)
        {
            return  _context.Jobs.Where(x=>x.Status == JobStatus.created).Include(u=>u.User).ToList();
        }

        public JobEntity GetJobWithId(int Id)
        {
            return  _context.Jobs.Include(u=>u.User).FirstOrDefault(x=>x.Id == Id);
        }
    }

}