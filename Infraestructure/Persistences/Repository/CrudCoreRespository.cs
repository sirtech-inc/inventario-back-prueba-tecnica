
using Domain.Entities;
using Infraestructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infraestructure.Core.Repositories
{
    public class CrudCoreRespository<T, ID> : ICrudCoreRespository<T, ID> where T : class
    {
        public readonly InventarioContext _context;

        public CrudCoreRespository(InventarioContext context)
        {
            _context = context;
        }

        public virtual async Task<IReadOnlyList<T>> FindAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> FindByIdAsync(ID id)
        {
            return await _context.Set<T>().FindAsync(id); ;
        }

        public virtual async Task<T> SaveAsync(T entity)
        {
            EntityState state = _context.Entry(entity).State;

            if (state != EntityState.Unchanged)
            {

                _ = state switch
                {
                    EntityState.Detached => _context.Set<T>().Add(entity),
                    EntityState.Modified => _context.Set<T>().Update(entity),
                };

                await _context.SaveChangesAsync();


            }


            return entity;
        }

        public virtual async Task ForceUpdateWithoutRelationsAsync(T entity)
        {
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _context.Attach(entity);
            }

            entry.State = EntityState.Modified;

            foreach (var navigation in entry.Navigations)
            {
                if (navigation.CurrentValue != null)
                {
                    _context.Entry(navigation.CurrentValue).State = EntityState.Unchanged;
                }
            }

            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdatePrincipalAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task SaveRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            await _context.SaveChangesAsync();
        }


        public async Task<string> GenerateCodeAsync(
            string prefix,
            string codePropertyName = "Codigo",
            int totalLength = 3)
        {
            var set = _context.Set<T>();

            var codes = await set
                .AsNoTracking()
                .Select(e => EF.Property<string>(e, codePropertyName))
                .Where(c => c.StartsWith(prefix))
                .ToListAsync();

            int maxNumber = 0;

            foreach (var code in codes)
            {
                var numberPart = code.Substring(prefix.Length);

                if (int.TryParse(numberPart, out int number))
                {
                    if (number > maxNumber)
                        maxNumber = number;
                }
            }

            int nextNumber = maxNumber + 1;

            string paddedNumber = nextNumber.ToString().PadLeft(totalLength, '0');

            return $"{prefix}{paddedNumber}";
        }

        public virtual async Task<bool> ExistsByPropertyAsync(
            string propertyName,
            string value,
            ID? excludeId = default)
        {
            var query = _context.Set<T>().AsQueryable();

            query = query.Where(e =>
                EF.Functions.Collate(
                    EF.Property<string>(e, propertyName),
                    "Latin1_General_CI_AI"
                ) == value);

            if (excludeId != null && !excludeId.Equals(default(ID)))
            {
                query = query.Where(e =>
                    !EF.Property<ID>(e, "Id")!.Equals(excludeId));
            }

            return await query.AnyAsync();
        }

        public virtual async Task<T?> FindByCodeAsync(
            string code,
            string codePropertyName = "Codigo")
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    EF.Functions.Collate(
                        EF.Property<string>(e, codePropertyName),
                        "Latin1_General_CI_AI"
                    ) == code);
        }

        public async Task<T?> FindByPropertyAsync(string propertyName, object value)
        {
            return await _context.Set<T>()
                .Where(e => EF.Property<object>(e, propertyName).Equals(value))
                .FirstOrDefaultAsync();
        }
        public async Task<IReadOnlyList<T>> FindAllByPropertyAsync(string propertyName, object value)
        {
            return await _context.Set<T>()
                .Where(e => EF.Property<object>(e, propertyName).Equals(value))
                .ToListAsync();
        }
    }
}

