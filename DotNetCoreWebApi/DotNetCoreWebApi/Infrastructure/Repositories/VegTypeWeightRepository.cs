using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebApi.Infrastructure.Repositories
{
    public class VegTypeWeightRepository : Repository<VegTypeWeight>, IVegTypeWeightRepository
    {
        public VegTypeWeightRepository(ApplicationDBContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VegTypeWeight>> GetActiveTypesAsync()
        {
            return await _context.Set<VegTypeWeight>()
                .Where(tw => tw.IsActive)
                .OrderBy(tw => tw.Name)
                .ToListAsync();
        }
    }
}
