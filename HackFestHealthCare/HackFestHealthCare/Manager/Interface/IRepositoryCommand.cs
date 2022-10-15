using HackFestHealthCare.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HackFestHealthCare.Manager.Interface
{
    public interface IRepositoryCommand<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        void Save();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        EntityEntry<TEntity> Insert(TEntity entity);
        Task<EntityEntry<TEntity>> InsertAsync(TEntity entity);
        TPrimaryKey InsertAndGetId(TEntity entity);
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);
        EntityEntry<TEntity> InsertOrUpdate(TEntity entity);
        Task<EntityEntry<TEntity>> InsertOrUpdateAsync(TEntity entity);
        TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);
        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);
        EntityEntry<TEntity> Update(TEntity entity);
        Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity);
        TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);
        Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);
        void Delete(TEntity entity);
        Task DeleteAsync(TEntity entity);
        void Delete(TPrimaryKey id);
        Task DeleteAsync(TPrimaryKey id);
        public void HardDelete(TEntity entity);
        Task<EntityEntry<TEntity>> InsertOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity);  
    }
}
