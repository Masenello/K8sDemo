using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using K8sCore.Entities;
using K8sCore.Specifications;

namespace K8sCore.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        IEnumerable<T> FindWithSpecificationPattern(ISpecification<T> specification = null);
    }
}