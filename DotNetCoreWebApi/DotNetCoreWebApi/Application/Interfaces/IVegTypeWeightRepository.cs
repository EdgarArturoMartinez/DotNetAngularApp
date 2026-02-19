using DotNetCoreWebApi.Application.Entities;

namespace DotNetCoreWebApi.Application.Interfaces
{
    public interface IVegTypeWeightRepository : IRepository<VegTypeWeight>
    {
        Task<IEnumerable<VegTypeWeight>> GetActiveTypesAsync();
    }
}
