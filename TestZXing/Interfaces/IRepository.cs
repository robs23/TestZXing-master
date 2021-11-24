using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        Task<List<T>> GetAsync(); 
        List<T> Get();

        Task<T> GetAsync(int id);
        T Get(int id);

        Task<List<T>> GetAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);
        List<T> Get<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        T Get(Expression<Func<T, bool>> predicate);

        AsyncTableQuery<T> AsQueryableAsync();
        TableQuery<T> AsQueryable();

        Task<int> InsertAsync(T entity);
        int Insert(T entity);

        int InsertOrReplace(T entity);

        Task<int> UpdateAsync(T entity);
        int Update(T entity);

        Task<int> DeleteAsync(T entity);
        int Delete(T entity);

    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private SQLiteAsyncConnection asyncDb;
        private SQLiteConnection db;
        public bool AsycRepository { get; set; }

        public Repository(SQLiteAsyncConnection db)
        {
            this.asyncDb = db;
            this.AsycRepository = true;
        }

        public Repository(SQLiteConnection db)
        {
            this.db = db;
            this.AsycRepository = false;
        }

        public AsyncTableQuery<T> AsQueryableAsync() =>
            asyncDb.Table<T>();

        public TableQuery<T> AsQueryable() =>
            db.Table<T>();

        public async Task<List<T>> GetAsync() =>
            await asyncDb.Table<T>().ToListAsync();

        public List<T> Get()
        {
            return db.Table<T>().ToList();
        }

        public async Task<List<T>> GetAsync<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
        {
            var query = asyncDb.Table<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = query.OrderBy<TValue>(orderBy);

            return await query.ToListAsync();
        }

        public List<T> Get<TValue>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TValue>> orderBy = null)
        {
            var query = db.Table<T>();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = query.OrderBy<TValue>(orderBy);

            return query.ToList();
        }

        public async Task<T> GetAsync(int id) =>
             await asyncDb.FindAsync<T>(id);

        public T Get(int id) =>
            db.Find<T>(id);

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate) =>
            await asyncDb.FindAsync<T>(predicate);

        public T Get(Expression<Func<T, bool>> predicate) =>
            db.Find<T>(predicate);

        public async Task<int> InsertAsync(T entity) =>
             await asyncDb.InsertAsync(entity);

        public int Insert(T entity) =>
             db.Insert(entity);

        public int InsertOrReplace(T entity) =>
            db.InsertOrReplace(entity);

        public async Task<int> UpdateAsync(T entity) =>
             await asyncDb.UpdateAsync(entity);

        public int Update(T entity) =>
             db.Update(entity);

        public async Task<int> DeleteAsync(T entity) =>
             await asyncDb.DeleteAsync(entity);
        public int Delete(T entity) =>
             db.Delete(entity);
    }
}
