using Domain.Entities;
using Infraestructure.Core.Repositories;
using Infraestructure.Persistences.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistences.Repository
{
    public class UserRepository : CrudCoreRespository<User, int>, IUserRepository
    {
        private readonly InventarioContext _dbContext;

        public UserRepository(InventarioContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _dbContext.Set<User>()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
