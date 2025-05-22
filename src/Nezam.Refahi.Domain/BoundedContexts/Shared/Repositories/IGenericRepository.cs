using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Shared.Repositories
{
    /// <summary>
    /// Generic repository interface that provides standard operations for domain entities.
    /// Adheres to Domain-Driven Design principles by focusing on the domain model.
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        #region Basic CRUD Operations

        /// <summary>
        /// Gets entity by id
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <returns>Entity or null if not found</returns>
        Task<TEntity?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>Collection of all entities</returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Deletes an entity by id
        /// </summary>
        /// <param name="id">Entity identifier</param>
        /// <returns>True if deleted, false if entity not found</returns>
        Task<bool> DeleteAsync(Guid id);

        #endregion

        #region Query Operations with Expressions

        /// <summary>
        /// Finds entities that match a specified condition
        /// </summary>
        /// <param name="predicate">The condition to match</param>
        /// <returns>Collection of matching entities</returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Finds a single entity that matches a specified condition
        /// </summary>
        /// <param name="predicate">The condition to match</param>
        /// <returns>Matching entity or null if not found</returns>
        Task<TEntity?> FindOneAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets a single entity that matches a specified condition, throws exception if not found
        /// </summary>
        /// <param name="predicate">The condition to match</param>
        /// <returns>Matching entity</returns>
        /// <exception cref="InvalidOperationException">Thrown when entity is not found</exception>
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Checks if any entity matches the specified condition
        /// </summary>
        /// <param name="predicate">The condition to match</param>
        /// <returns>True if any entity matches, false otherwise</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Counts entities that match a specified condition
        /// </summary>
        /// <param name="predicate">The condition to match (optional)</param>
        /// <returns>Count of matching entities</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

        #endregion

        #region Advanced Queries

        /// <summary>
        /// Gets a paginated list of entities
        /// </summary>
        /// <param name="predicate">Filter condition (optional)</param>
        /// <param name="orderBy">Order function (optional)</param>
        /// <param name="skip">Number of entities to skip</param>
        /// <param name="take">Number of entities to take</param>
        /// <param name="isDescending">Whether to order in descending direction</param>
        /// <returns>Collection of paginated entities</returns>
        Task<IEnumerable<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Expression<Func<TEntity, object>>? orderBy = null,
            int skip = 0,
            int take = 20,
            bool isDescending = false);

        /// <summary>
        /// Gets entities with specific included properties (eager loading)
        /// </summary>
        /// <param name="includes">List of property expressions to include</param>
        /// <returns>Collection of entities with included properties</returns>
        Task<IEnumerable<TEntity>> GetWithIncludesAsync(params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Gets entities with specific included properties filtered by condition
        /// </summary>
        /// <param name="predicate">Filter condition</param>
        /// <param name="includes">List of property expressions to include</param>
        /// <returns>Collection of filtered entities with included properties</returns>
        Task<IEnumerable<TEntity>> GetWithIncludesAsync(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes);

        #endregion

        #region Batch Operations

        /// <summary>
        /// Adds a collection of entities
        /// </summary>
        /// <param name="entities">Entities to add</param>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates a collection of entities
        /// </summary>
        /// <param name="entities">Entities to update</param>
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes entities matching the specified condition
        /// </summary>
        /// <param name="predicate">Condition to match entities for deletion</param>
        /// <returns>Number of entities deleted</returns>
        Task<int> DeleteRangeAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
