namespace Nezam.Refahi.Shared.Application.Common.Contracts;

/// <summary>
/// Generic mapper interface for converting between entities and DTOs
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TDto">The DTO type</typeparam>
public interface IMapper<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    /// <summary>
    /// Maps from entity to DTO
    /// </summary>
    /// <param name="entity">The source entity</param>
    /// <returns>Mapped DTO</returns>
    TDto MapFrom(TEntity entity);

    /// <summary>
    /// Maps from DTO to entity
    /// </summary>
    /// <param name="dto">The source DTO</param>
    /// <returns>Mapped entity</returns>
    TEntity MapTo(TDto dto);
}

/// <summary>
/// Static mapper interface for DTOs that implement static mapping methods
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TDto">The DTO type that implements static mapping</typeparam>
public interface IStaticMapper<TEntity, TDto>
    where TEntity : class
    where TDto : class, IStaticMapper<TEntity, TDto>
{
    /// <summary>
    /// Static method to map from entity to DTO
    /// </summary>
    /// <param name="entity">The source entity</param>
    /// <returns>Mapped DTO</returns>
    static abstract TDto MapFrom(TEntity entity);

    /// <summary>
    /// Static method to map from DTO to entity
    /// </summary>
    /// <param name="dto">The source DTO</param>
    /// <returns>Mapped entity</returns>
    static abstract TEntity MapTo(TDto dto);
}
