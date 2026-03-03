using DotNetCoreWebApi.Application.Interfaces;
using DotNetCoreWebApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DotNetCoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VegTypeWeightsController : ControllerBase
    {
        private readonly IVegTypeWeightService _service;
        private readonly ILogger<VegTypeWeightsController> _logger;

        public VegTypeWeightsController(IVegTypeWeightService service, ILogger<VegTypeWeightsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/VegTypeWeights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VegTypeWeightDto>>> GetAllVegTypeWeights()
        {
            try
            {
                var typeWeights = await _service.GetAllAsync();
                _logger.LogDebug("Retrieved {Count} type weights", typeWeights.Count());
                return Ok(typeWeights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all type weights");
                return StatusCode(500, new { message = "An error occurred retrieving type weights" });
            }
        }

        // GET: api/VegTypeWeights/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VegTypeWeightBasicDto>>> GetActiveTypes()
        {
            try
            {
                var typeWeights = await _service.GetActiveTypesAsync();
                return Ok(typeWeights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active type weights");
                return StatusCode(500, new { message = "An error occurred retrieving active type weights" });
            }
        }

        // GET: api/VegTypeWeights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VegTypeWeightDto>> GetVegTypeWeight(int id)
        {
            try
            {
                var typeWeight = await _service.GetByIdAsync(id);
                if (typeWeight == null)
                {
                    _logger.LogWarning("Type weight not found: {TypeWeightId}", id);
                    return NotFound();
                }
                return Ok(typeWeight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving type weight {TypeWeightId}", id);
                return StatusCode(500, new { message = "An error occurred retrieving the type weight" });
            }
        }

        // POST: api/VegTypeWeights
        [HttpPost]
        public async Task<ActionResult<VegTypeWeightDto>> CreateVegTypeWeight(VegTypeWeightCreateUpdateDto dto)
        {
            try
            {
                var typeWeight = await _service.CreateAsync(dto);
                _logger.LogInformation("Type weight created: {Name} (ID: {TypeWeightId})", typeWeight.Name, typeWeight.IdTypeWeight);
                return CreatedAtAction(nameof(GetVegTypeWeight), new { id = typeWeight.IdTypeWeight }, typeWeight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating type weight {Name}", dto.Name);
                return StatusCode(500, new { message = "An error occurred creating the type weight" });
            }
        }

        // PUT: api/VegTypeWeights/5
        [HttpPut("{id}")]
        public async Task<ActionResult<VegTypeWeightDto>> UpdateVegTypeWeight(int id, VegTypeWeightCreateUpdateDto dto)
        {
            try
            {
                var typeWeight = await _service.UpdateAsync(id, dto);
                _logger.LogInformation("Type weight updated: {TypeWeightId}", id);
                return Ok(typeWeight);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Update failed - type weight not found: {TypeWeightId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating type weight {TypeWeightId}", id);
                return StatusCode(500, new { message = "An error occurred updating the type weight" });
            }
        }

        // DELETE: api/VegTypeWeights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVegTypeWeight(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                _logger.LogInformation("Type weight deleted: {TypeWeightId}", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Delete failed - type weight not found: {TypeWeightId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting type weight {TypeWeightId}", id);
                return StatusCode(500, new { message = "An error occurred deleting the type weight" });
            }
        }
    }
}
