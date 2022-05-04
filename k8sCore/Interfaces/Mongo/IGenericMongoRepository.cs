using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using K8sCore.Entities.Ef;
using K8sCore.Entities.Mongo;
using K8sCore.Specifications;

namespace K8sCore.Interfaces.Mongo
{
    public interface IGenericMongoRepository<T> where T : BaseMongoEntity
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        List<T> Find(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task Remove(T entity);
        Task UpdateAsync(string id, T updatedT);
    }
}