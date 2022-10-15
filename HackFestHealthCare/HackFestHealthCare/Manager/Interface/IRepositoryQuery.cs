using HackFestHealthCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HackFestHealthCare.Manager.Interface
{
    public interface IRepositoryQuery<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> GetAllIncludeInactive();
        List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllListAsync();
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
        TEntity Get(TPrimaryKey id);
        Task<TEntity> GetAsync(TPrimaryKey id);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(TPrimaryKey id);
        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Load(TPrimaryKey id);
        int Count();
        Task<int> CountAsync();
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        long LongCount();
        Task<long> LongCountAsync();
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> SelectQuery(string query, params object[] parameters);
        void ExecuteStoreprocedure(string storedProcedureNameAndParameterPlaceholder, params object[] parameters);
    }
}
