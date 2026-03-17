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
    }
}
