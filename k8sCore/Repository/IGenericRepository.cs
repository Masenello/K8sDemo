using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using k8sCore.Entities;
using k8sCore.Specifications;

namespace k8sCore.Repository
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