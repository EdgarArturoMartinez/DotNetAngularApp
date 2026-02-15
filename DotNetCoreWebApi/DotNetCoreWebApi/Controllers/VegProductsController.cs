using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace DotNetCoreWebApi.Controllers
{
    [Route("api/vegproducts")]
    public class VegProductsController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        public VegProductsController(ApplicationDBContext context)
        {
            this.context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<VegProducts>>> GetAsync()
        //{
        //    var vegProducts = await context.VegProducts.ToListAsync();
        //    return vegProducts.Select(p => new VegProducts
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        Price = p.Price
        //    }).ToList();
        //}

        [HttpGet]
        public async Task<ActionResult<List<VegProducts>>> GetAllAsync()
        {
            return await context.VegProducts.ToListAsync();
        }

        //[HttpGet]
        //public async Task<List<VegProducts>> GetAsync()
        //{
        //    var vegProducts = await context.VegProducts.ToListAsync();
        //    return vegProducts.Select(p => new VegProducts
        //    {
        //        Id = p.Id,
        //        Name = p.Name,
        //        Price = p.Price
        //    }).ToList();
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<VegProducts>> GetByIdAsync(int id)
        {
            var vegProduct = await context.VegProducts.FindAsync(id);
            if (vegProduct == null)
                return NotFound();
            return vegProduct;
        }


        [HttpPost]
        public async Task<ActionResult<VegProducts>> CreateAsync([FromBody] VegProducts vegProduct)
        {
            context.VegProducts.Add(vegProduct);
            await context.SaveChangesAsync();
            return Ok(vegProduct);  // Simple 200 OK response
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] VegProducts vegProduct)
        {
            var existing = await context.VegProducts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = vegProduct.Name;
            existing.Price = vegProduct.Price;
            await context.SaveChangesAsync();
            return Ok(existing);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var vegProduct = await context.VegProducts.FindAsync(id);
            if (vegProduct == null)
                return NotFound();

            context.VegProducts.Remove(vegProduct);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
