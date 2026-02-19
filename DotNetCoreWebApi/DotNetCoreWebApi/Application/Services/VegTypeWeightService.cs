using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;

namespace DotNetCoreWebApi.Application.Services
{
    public class VegTypeWeightService : IVegTypeWeightService
    {
        private readonly IVegTypeWeightRepository _repository;

        public VegTypeWeightService(IVegTypeWeightRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VegTypeWeightDto>> GetAllAsync()
        {
            var typeWeights = await _repository.GetAllAsync();
            return typeWeights.Select(tw => new VegTypeWeightDto
            {
                IdTypeWeight = tw.Id,
                Name = tw.Name,
                AbbreviationWeight = tw.AbbreviationWeight,
                Description = tw.Description,
                IsActive = tw.IsActive,
                CreatedAt = tw.CreatedAt
            });
        }

        public async Task<IEnumerable<VegTypeWeightBasicDto>> GetActiveTypesAsync()
        {
            var typeWeights = await _repository.GetActiveTypesAsync();
            return typeWeights.Select(tw => new VegTypeWeightBasicDto
            {
                IdTypeWeight = tw.Id,
                Name = tw.Name,
                AbbreviationWeight = tw.AbbreviationWeight
            });
        }

        public async Task<VegTypeWeightDto?> GetByIdAsync(int id)
        {
            var typeWeight = await _repository.GetByIdAsync(id);
            if (typeWeight == null) return null;

            return new VegTypeWeightDto
            {
                IdTypeWeight = typeWeight.Id,
                Name = typeWeight.Name,
                AbbreviationWeight = typeWeight.AbbreviationWeight,
                Description = typeWeight.Description,
                IsActive = typeWeight.IsActive,
                CreatedAt = typeWeight.CreatedAt
            };
        }

        public async Task<VegTypeWeightDto> CreateAsync(VegTypeWeightCreateUpdateDto dto)
        {
            var typeWeight = new VegTypeWeight
            {
                Name = dto.Name,
                AbbreviationWeight = dto.AbbreviationWeight,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Now
            };

            await _repository.AddAsync(typeWeight);

            return new VegTypeWeightDto
            {
                IdTypeWeight = typeWeight.Id,
                Name = typeWeight.Name,
                AbbreviationWeight = typeWeight.AbbreviationWeight,
                Description = typeWeight.Description,
                IsActive = typeWeight.IsActive,
                CreatedAt = typeWeight.CreatedAt
            };
        }

        public async Task<VegTypeWeightDto> UpdateAsync(int id, VegTypeWeightCreateUpdateDto dto)
        {
            var typeWeight = await _repository.GetByIdAsync(id);
            if (typeWeight == null)
                throw new KeyNotFoundException($"VegTypeWeight with ID {id} not found");

            typeWeight.Name = dto.Name;
            typeWeight.AbbreviationWeight = dto.AbbreviationWeight;
            typeWeight.Description = dto.Description;
            typeWeight.IsActive = dto.IsActive;

            await _repository.UpdateAsync(typeWeight);

            return new VegTypeWeightDto
            {
                IdTypeWeight = typeWeight.Id,
                Name = typeWeight.Name,
                AbbreviationWeight = typeWeight.AbbreviationWeight,
                Description = typeWeight.Description,
                IsActive = typeWeight.IsActive,
                CreatedAt = typeWeight.CreatedAt
            };
        }

        public async Task DeleteAsync(int id)
        {
            var typeWeight = await _repository.GetByIdAsync(id);
            if (typeWeight == null)
                throw new KeyNotFoundException($"VegTypeWeight with ID {id} not found");

            await _repository.DeleteAsync(typeWeight);
        }
    }
}
