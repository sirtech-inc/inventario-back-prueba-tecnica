using Domain.Entities;
using Infraestructure.Core.Repositories;
using Infraestructure.Persistences.Interfaces;

namespace Infraestructure.Persistences.Repository
{
    public class MovimientoRepository : CrudCoreRespository<Movimiento, int>, IMovimientoRepository
    {
        public MovimientoRepository(InventarioContext context) : base(context)
        {
        }
    }
}

