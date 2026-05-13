using Microsoft.EntityFrameworkCore;
using RentACar.Core.Entities;
using System.Linq.Expressions;

namespace RentACar.Core.DataAccess.EntityFramework
{
    public class EfEntityRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity  : class, IEntity, new()
        where TContext : DbContext, new()
    {
        // Injected context — non-null when created via DI
        private readonly TContext? _context;

        public EfEntityRepositoryBase() { }

        public EfEntityRepositoryBase(TContext context)
        {
            _context = context;
        }

        // If a DI context was provided use it directly (no using — DI manages lifetime).
        // Otherwise fall back to creating a short-lived context (manages its own lifetime).
        private IDisposable? _ownedContext;

        private TContext AcquireContext()
        {
            if (_context != null) return _context;
            var ctx = new TContext();
            _ownedContext = ctx;
            return ctx;
        }

        public void Add(TEntity entity)
        {
            var ctx = AcquireContext();
            try
            {
                ctx.Entry(entity).State = EntityState.Added;
                ctx.SaveChanges();
            }
            finally { DisposeOwned(); }
        }

        public void Delete(TEntity entity)
        {
            var ctx = AcquireContext();
            try
            {
                ctx.Entry(entity).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
            finally { DisposeOwned(); }
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> filter)
        {
            var ctx = AcquireContext();
            try   { return ctx.Set<TEntity>().SingleOrDefault(filter); }
            finally { DisposeOwned(); }
        }

        public List<TEntity> GetAll(Expression<Func<TEntity, bool>>? filter = null)
        {
            var ctx = AcquireContext();
            try
            {
                return filter == null
                    ? ctx.Set<TEntity>().ToList()
                    : ctx.Set<TEntity>().Where(filter).ToList();
            }
            finally { DisposeOwned(); }
        }

        public void Update(TEntity entity)
        {
            var ctx = AcquireContext();
            try
            {
                ctx.Entry(entity).State = EntityState.Modified;
                ctx.SaveChanges();
            }
            finally { DisposeOwned(); }
        }

        private void DisposeOwned()
        {
            _ownedContext?.Dispose();
            _ownedContext = null;
        }
    }
}