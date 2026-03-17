using Domain;
using Domain.Entities;
using Infraestructure.Core.Repositories;
using Infraestructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistences.Repository
{
    public class ProductRepository : CrudCoreRespository<Product, int>, IProductRepository
    {
        private readonly InventarioContext _dbContext;
        public ProductRepository(InventarioContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<PaginadoResponse<Product>> BusquedaPaginado(PaginationRequest dto)
        {
            var contex = _dbContext.Set<Product>()
                     .AsSplitQuery()
                     .AsQueryable();


            if (!string.IsNullOrWhiteSpace(dto.Sort))
            {
                var ColumnsOrder = dto.Sort.Split(".");

                var column = ColumnsOrder[0];
                var order = ColumnsOrder[1];

                contex = column switch
                {
                    "name" => order == "desc" ? contex.OrderByDescending(p => p.Name) : contex.OrderBy(p => p.Name),
                };

            }


            if (dto.Filters != null && dto.Filters.Length > 0)
            {
                foreach (var filter in dto.Filters)
                {
                    var id_value = filter.Split(":");

                    var id = id_value[0];
                    var value = id_value[1];
                    
                    if (id == "name") contex = contex.Where(p => p.Name.Contains(value));

                }
            }

            var take = dto.Take ?? 5;
            var page = dto.Page ?? 1;
            var skip = (page - 1) * take;

            var data = await contex.Skip(skip).Take(take).ToListAsync();
            var total = await contex.CountAsync();

            var meta = new Meta
            {
                Page = dto.Page,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling((double)total / take)
            };


            PaginadoResponse<Product> response = new(data, meta);

            return response;
        }
    }
}
