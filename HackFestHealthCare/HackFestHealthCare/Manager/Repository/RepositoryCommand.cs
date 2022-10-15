using HackFestHealthCare.Context;
using HackFestHealthCare.Manager.Interface;
using HackFestHealthCare.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HackFestHealthCare.Manager.Repository
{
    public class RepositoryCommand<TEntity, TPrimaryKey> : IRepositoryCommand<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly HealthCareContext _context;
        public RepositoryCommand(HealthCareContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// get the current Entity
        /// <returns><see cref="DbSet{TEntity}"/></returns>
        /// </summary>

        public virtual DbSet<TEntity> Table => _context.Set<TEntity>();

        public void Save()
        {
            _context.SaveChanges();
        }

        //public TPrimaryKey SaveChanges()
        //{
        //    return _context.SaveChanges()(TPrimaryKey);
        //}

        //public Task<TPrimaryKey> SaveChangesAsync()
        //{
        //    return _context.SaveChangesAsync();
        //}

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }


        public EntityEntry<TEntity> Insert(TEntity entity)
        {
            return Table.Add(entity);
        }

        public Task<EntityEntry<TEntity>> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Table.Add(entity));
        }



        public TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity).Entity;
            if (entity.IsTransient())
            {
                _context.SaveChanges();
            }

            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            var ent = await InsertAsync(entity);
            entity = ent.Entity;
            if (entity.IsTransient())
            {
                await _context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            entity = InsertOrUpdate(entity).Entity;
            if (entity.IsTransient())
            {
                _context.SaveChanges();
            }

            return entity.Id;
        }

        public async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            var ent = await InsertOrUpdateAsync(entity);
            entity = ent.Entity;
            if (entity.IsTransient())
            {
                await _context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public EntityEntry<TEntity> Update(TEntity entity)
        {
            AttachIfNot(entity);
            var ent = _context.Entry(entity);
            ent.State = EntityState.Modified;
            return ent;
        }

        public Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            var ent = _context.Entry(entity);
            ent.State = EntityState.Modified;
            return Task.FromResult(ent);
        }

        public void Delete(TEntity entity)
        {
            Update(entity.Id, x => x.IsDeleted = true);
        }

        public void HardDelete(TEntity entity)
        {
            _context.Remove(entity);
        }


        public TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            var entity = Table.Find(id);
            updateAction(entity);
            return entity;
        }

        public void Delete(TPrimaryKey id)
        {
            Update(id, x => x.IsDeleted = true);
        }

        public Task DeleteAsync(TPrimaryKey id)
        {
            return Task.Run(() => Delete(id));

        }


        public async Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction)
        {
            var entity = await Table.FindAsync(id);
            await updateAction(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Func<TEntity, Task> updateAction)
        {
            var entity = await Table.FirstOrDefaultAsync(predicate);
            await updateAction(entity);
            return entity;
        }


        public virtual Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.FromResult(0);
        }


        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }

        #region MyCommandRegion
        public async Task<EntityEntry<TEntity>> InsertOrUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
        {
            return !Table.Any(predicate) ? await InsertAsync(entity) : await UpdateAsync(entity);
        }


        public virtual EntityEntry<TEntity> InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                ? Insert(entity)
                : Update(entity);
        }

        public virtual async Task<EntityEntry<TEntity>> InsertOrUpdateAsync(TEntity entity)
        {
            return entity.IsTransient()
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }
        #endregion
    }
}
