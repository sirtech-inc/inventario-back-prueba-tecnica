using Microsoft.EntityFrameworkCore.Storage;

namespace Infraestructure.Persistences.Interfaces
{
    public interface ICrudCoreRespository<T, ID>
    {
        Task<IReadOnlyList<T>> FindAllAsync();
        Task<T?> FindByIdAsync(ID id);
        Task<T> SaveAsync(T entity);
        Task ForceUpdateWithoutRelationsAsync(T entity);
        Task SaveRangeAsync(IEnumerable<T> entities);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task UpdatePrincipalAsync(T entity);
        Task SaveChangesAsync();
        Task<T> AddAsync(T entity);

        Task<string> GenerateCodeAsync(
            string prefix,
            string codePropertyName = "Codigo",
            int totalLength = 3);

        Task<bool> ExistsByPropertyAsync(
               string propertyName,
               string value,
               ID? excludeId = default);

        Task<T?> FindByCodeAsync(
            string code,
            string codePropertyName = "Codigo");

        Task<T?> FindByPropertyAsync(string propertyName, object value);
        Task<IReadOnlyList<T>> FindAllByPropertyAsync(string propertyName, object value);

    }
}
