using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using K8sDataMongo.Settings;
using System.Threading.Tasks;
using K8sCore.Interfaces.Mongo;
using K8sCore.Entities.Mongo;

namespace K8sDataMongo.Repository
{
    public class GenericMongoRepository<T> : IGenericMongoRepository<T> where T : BaseMongoEntity 
    {
        protected readonly IMongoCollection<T>  _collection;
        protected readonly IMongoDatabase  _database;
        protected readonly MongoClient  _client;
        // public GenericRepository(IOptions<MongoDatabaseSettings> mongoDatabaseSettings)
        public GenericMongoRepository()
        {
            //TODO Manage with IOptions!!!!
            _client = new MongoClient(
            NetworkSettings.DatabaseConnectionStringResolver());
            

            _database = _client.GetDatabase(
            "TestMongo");

            _collection = _database.GetCollection<T>(typeof(T).Name);
        }

        public Task<T> GetByIdAsync(string id)
        {
            return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<T>> GetAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public List<T> Find(Expression<Func<T, bool>> expression)
        {
            return _collection.AsQueryable().Where(expression).ToList();
        }

        public Task AddAsync(T entity)
        {
            return _collection.InsertOneAsync(entity);
            
        }


        public Task Remove(T entity)
        {
            return _collection.DeleteOneAsync(x => x.Id == entity.Id);
        }


        public async Task UpdateAsync(string id, T updatedT) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, updatedT);

    }
}