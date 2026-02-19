using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Interfaces
{
    public interface IVegTypeWeightService
    {
        Task<IEnumerable<VegTypeWeightDto>> GetAllAsync();
        Task<IEnumerable<VegTypeWeightBasicDto>> GetActiveTypesAsync();
        Task<VegTypeWeightDto?> GetByIdAsync(int id);
        Task<VegTypeWeightDto> CreateAsync(VegTypeWeightCreateUpdateDto dto);
        Task<VegTypeWeightDto> UpdateAsync(int id, VegTypeWeightCreateUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
