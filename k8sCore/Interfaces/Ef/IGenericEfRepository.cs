using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using K8sCore.Entities.Ef;
using K8sCore.Specifications;

namespace K8sCore.Interfaces.Ef
{
    public interface IGenericEfRepository<T> where T : BaseEfEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>>  FindAsync(Expression<Func<T, bool>> expression);
        Task<T> FindFirstOrDefaultAsync(Expression<Func<T, bool>> expression);
        void AddAsync(T entity);
        void AddRangeAsync(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<List<T>> FindWithSpecificationPatternAsync(ISpecification<T> specification = null);
    }
}