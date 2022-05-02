using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using K8sCore.Entities;
using K8sCore.Interfaces;
using K8sCore.Specifications;
using k8sData;
using K8sData.Data;

namespace K8sData
{
    public class GenericRepository<T> : IGenericRepository<T>, IDisposable where T : BaseEntity 
    {
        protected readonly DataContext  _context;
        public GenericRepository(DataContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public IEnumerable<T> FindWithSpecificationPattern(ISpecification<T> specification = null)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), specification);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}