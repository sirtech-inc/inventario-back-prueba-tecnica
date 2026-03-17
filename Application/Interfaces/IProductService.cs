using Application.Core.Services.Interfaces;
using Application.Dtos.Product;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService : ICurdCoreService<ProductDto, ProductRequestDto, string>
    {
        Task<PaginadoResponse<ProductDto>> BusquedaPaginado(PaginationRequest dto);
        Task<OperationResult<ProductDto>> AjustarStockAsync(string id, StockMoveRequestDto request);
        Task<OperationResult<bool>> DeleteAsync(string id);
        Task<PaginadoResponse<ProductDto>> BusquedaPaginadoStockBajo(PaginationRequest dto);
        Task<IReadOnlyList<ProductDto>> GetStockBajoAsync(int maxStock = 5);
    }
}
