using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories;

/// <summary>
/// Generic repository interface that provides standard operations for domain entities.
/// Adheres to Domain-Driven Design principles by focusing on the domain model.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IGenericRepository<TEntity> where TEntity : BaseEntity
{
  #region Basic CRUD Operations

  Task<TEntity?> GetByIdAsync(Guid id);
  Task<IEnumerable<TEntity>> GetAllAsync();
  Task AddAsync(TEntity entity, bool saveChanges = false);
  Task UpdateAsync(TEntity entity, bool saveChanges = false);
  Task<bool> DeleteAsync(Guid id, bool saveChanges = false);

  #endregion

  #region Query Operations with Expressions

  Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
  Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);
  Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
  Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
  Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

  #endregion

  #region Advanced Queries

  Task<IEnumerable<TEntity>> GetPagedAsync(
      Expression<Func<TEntity, bool>>? predicate = null,
      Expression<Func<TEntity, object>>? orderBy = null,
      int skip = 0,
      int take = 20,
      bool isDescending = false);

  Task<IEnumerable<TEntity>> GetWithIncludesAsync(params Expression<Func<TEntity, object>>[] includes);

  Task<IEnumerable<TEntity>> GetWithIncludesAsync(
      Expression<Func<TEntity, bool>> predicate,
      params Expression<Func<TEntity, object>>[] includes);

  #endregion

  #region Batch Operations

  Task AddRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = false);
  Task UpdateRangeAsync(IEnumerable<TEntity> entities, bool saveChanges = false);
  Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate, bool saveChanges = false);

  #endregion
}
