using Domain;

namespace Application.Core.Services.Interfaces
{
    public interface ICurdCoreService<TDto, TSaveDto, ID>
    {
        Task<IReadOnlyList<TDto>> FindAllAsync();
        Task<TDto> FindByIdAsync(ID id);
        Task<OperationResult<TDto>> CreateAsync(TSaveDto saveDto);
        Task<OperationResult<TDto>> EditAsync(ID id, TSaveDto saveDto);
        Task<OperationResult<TDto>> DisabledAsync(ID id);
        Task<IReadOnlyList<TDto>> GetActivo();

    }
}
