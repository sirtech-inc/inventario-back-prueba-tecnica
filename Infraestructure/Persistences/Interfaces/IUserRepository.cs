using Domain.Entities;

namespace Infraestructure.Persistences.Interfaces
{
    public interface IUserRepository : ICrudCoreRespository<User, int>
    {
        Task<User?> FindByUsernameAsync(string username);
    }
}
