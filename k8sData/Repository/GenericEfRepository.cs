using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using K8sCore.Entities.Ef;
using K8sCore.Interfaces.Ef;
using K8sCore.Specifications;
using k8sData;
using K8sData.Data;
using Microsoft.EntityFrameworkCore;

namespace K8sData.Repository
{
    public class GenericEfRepository<T> : IGenericEfRepository<T> where T : BaseEfEntity 
    {
        protected readonly DataContext  _context;
        public GenericEfRepository(DataContext context)
        {
            _context = context;
        }

        public void AddAsync(T entity)
        {
            _context.Set<T>().AddAsync(entity);
        }

        public void AddRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRangeAsync(entities);
        }

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).ToListAsync();
        }

        public Task<T> FindFirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        public Task<List<T>> GetAllAsync()
        {
            return _context.Set<T>().ToListAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            return _context.Set<T>().FindAsync(id).AsTask();
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public Task<List<T>> FindWithSpecificationPatternAsync(ISpecification<T> specification = null)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification).ToListAsync();
        }

    }
}