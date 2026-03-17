using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Persistences.Interfaces
{
    public interface IProductRepository : ICrudCoreRespository<Product, int>
    {
        public Task<PaginadoResponse<Product>> BusquedaPaginado(PaginationRequest dto);
    }
}
