using System;
using System.Collections.Generic;
using System.Linq;
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