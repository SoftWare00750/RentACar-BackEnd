using Microsoft.EntityFrameworkCore;
using RentACar.Core.Entities;
using System.Linq.Expressions;

namespace RentACar.Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext, new()
    {
        private readonly TContext? _injectedContext;

        public EfEntityRepositoryBase() { }

        protected TContext GetContext()
        {
            return _injectedContext ?? new TContext();
        }

        public void Add(TEntity entity)
        {
            using var context = GetContext();
            context.Entry(entity).State = EntityState.Added;
            context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            using var context = GetContext();
            context.Entry(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> filter)
        {
            using var context = GetContext();
            return context.Set<TEntity>().SingleOrDefault(filter);
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null)
        {
            using var context = GetContext();
            return filter == null
                ? context.Set<TEntity>().ToList()
                : context.Set<TEntity>().Where(filter).ToList();
        }

        public void Update(TEntity entity)
        {
            using var context = GetContext();
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}