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

        public VegTypeWeightsController(IVegTypeWeightService service)
        {
            _service = service;
        }

        // GET: api/VegTypeWeights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VegTypeWeightDto>>> GetAllVegTypeWeights()
        {
            var typeWeights = await _service.GetAllAsync();
            return Ok(typeWeights);
        }

        // GET: api/VegTypeWeights/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<VegTypeWeightBasicDto>>> GetActiveTypes()
        {
            var typeWeights = await _service.GetActiveTypesAsync();
            return Ok(typeWeights);
        }

        // GET: api/VegTypeWeights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VegTypeWeightDto>> GetVegTypeWeight(int id)
        {
            var typeWeight = await _service.GetByIdAsync(id);
            if (typeWeight == null)
            {
                return NotFound();
            }
            return Ok(typeWeight);
        }

        // POST: api/VegTypeWeights
        [HttpPost]
        public async Task<ActionResult<VegTypeWeightDto>> CreateVegTypeWeight(VegTypeWeightCreateUpdateDto dto)
        {
            var typeWeight = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetVegTypeWeight), new { id = typeWeight.IdTypeWeight }, typeWeight);
        }

        // PUT: api/VegTypeWeights/5
        [HttpPut("{id}")]
        public async Task<ActionResult<VegTypeWeightDto>> UpdateVegTypeWeight(int id, VegTypeWeightCreateUpdateDto dto)
        {
            try
            {
                var typeWeight = await _service.UpdateAsync(id, dto);
                return Ok(typeWeight);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/VegTypeWeights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVegTypeWeight(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
