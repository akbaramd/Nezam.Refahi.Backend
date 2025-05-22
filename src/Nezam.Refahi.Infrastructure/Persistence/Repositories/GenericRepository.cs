using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

namespace Nezam.Refahi.Infrastructure.Persistence.Repositories;

/// <summary>
/// High-performance Generic Repository using Entity Framework Core
/// </summary>
/// <typeparam name="TEntity">Type of entity for this repository</typeparam>
public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<TEntity>();
    }

    public ApplicationDbContext AsDbContext() => _dbContext;
    public DbSet<TEntity> AsDbSet() => _dbSet;
    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable();
    
    #region Basic CRUD Operations

    public async Task<TEntity?> GetByIdAsync(Guid id) => await AsDbSet().FindAsync(id);

    public async Task<IEnumerable<TEntity>> GetAllAsync() => await AsDbSet().AsNoTracking().ToListAsync();

    public async Task AddAsync(TEntity entity, bool saveChanges = false)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await AsDbSet().AddAsync(entity);
        if (saveChanges) await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity, bool saveChanges = false)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _dbSet.Update(entity);
        if (saveChanges) await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id, bool saveChanges = false)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null) return false;
        _dbSet.Remove(entity);
        if (saveChanges) await _dbContext.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Query Operations

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await AsDbSet().Where(predicate).AsNoTracking().ToListAsync();

    public async Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate) =>
        await AsDbSet().AsNoTracking().FirstOrDefaultAsync(predicate);

    public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate) =>
        await AsDbSet().AsNoTracking().SingleAsync(predicate);

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate) =>
        await AsDbSet().AsNoTracking().AnyAsync(predicate);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null) =>
        predicate is null
            ? await AsDbSet().AsNoTracking().CountAsync()
            : await AsDbSet().AsNoTracking().CountAsync(predicate);

    #endregion

    #region Advanced Queries

    public async Task<IEnumerable<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        int skip = 0,
        int take = 20,
        bool isDescending = false)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        if (orderBy is not null)
            query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        return await query.Skip(skip).Take(take).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetWithIncludesAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetWithIncludesAsync(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate).AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);
        return await query.ToListAsync();
    }

    #endregion

    #region Batch Operations

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = false)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await AsDbSet().AddRangeAsync(entities);
        if (saveChanges) await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = false)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbSet.UpdateRange(entities);
        if (saveChanges) await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate, bool saveChanges = false)
    {
        var entities = await AsDbSet().Where(predicate).ToListAsync();
        if (!entities.Any()) return 0;
        _dbSet.RemoveRange(entities);
        if (saveChanges) await _dbContext.SaveChangesAsync();
        return entities.Count;
    }

    #endregion
}