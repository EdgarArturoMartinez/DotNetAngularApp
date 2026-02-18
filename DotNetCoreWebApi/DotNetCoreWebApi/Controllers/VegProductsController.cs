using DotNetCoreWebApi.Application.DBContext;
using DotNetCoreWebApi.Application.Entities;
using DotNetCoreWebApi.DTOs;
using DotNetCoreWebApi.Migrations;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VegProductDto>>> GetAllAsync()
        {
            var products = await context.VegProducts
                .Include(p => p.VegCategory)
                .AsNoTracking()
                .ToListAsync();

            var productDtos = products.Select(p => new VegProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                IdCategory = p.IdCategory,
                VegCategory = p.VegCategory == null ? null : new VegCategoryBasicDto
                {
                    IdCategory = p.VegCategory.IdCategory,
                    CategoryName = p.VegCategory.CategoryName,
                    Description = p.VegCategory.Description
                }
            }).ToList();

            return Ok(productDtos);
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
        public async Task<ActionResult<VegProductDto>> GetByIdAsync(int id)
        {
            var vegProduct = await context.VegProducts
                .Include(p => p.VegCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            
            if (vegProduct == null)
                return NotFound();

            var dto = new VegProductDto
            {
                Id = vegProduct.Id,
                Name = vegProduct.Name,
                Price = vegProduct.Price,
                Description = vegProduct.Description,
                StockQuantity = vegProduct.StockQuantity,
                IdCategory = vegProduct.IdCategory,
                VegCategory = vegProduct.VegCategory == null ? null : new VegCategoryBasicDto
                {
                    IdCategory = vegProduct.VegCategory.IdCategory,
                    CategoryName = vegProduct.VegCategory.CategoryName,
                    Description = vegProduct.VegCategory.Description
                }
            };

            return dto;
        }




        [HttpPost]
        public async Task<ActionResult<Application.Entities.VegProducts>> CreateAsync([FromBody] Application.Entities.VegProducts vegProduct)
        {
            context.VegProducts.Add(vegProduct);
            await context.SaveChangesAsync();
            return Ok(vegProduct);
        }


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateAsync(int id, [FromBody] Application.Entities.VegProducts vegProduct)
        //{
        //    var existing = await context.VegProducts.FindAsync(id);
        //    if (existing == null) return NotFound();

        //    existing.Name = vegProduct.Name;
        //    existing.Price = vegProduct.Price;
        //    await context.SaveChangesAsync();
        //    return Ok(existing);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVegproduct(int id, [FromBody] Application.Entities.VegProducts vegproduct)
        {
            if (id != vegproduct.Id)
                return BadRequest("ID mismatch");

            if (string.IsNullOrWhiteSpace(vegproduct.Name))
                return BadRequest("Product name is required");

            var existingProduct = await context.VegProducts.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

            // Update only the fields that should be updated
            existingProduct.Name = vegproduct.Name;
            existingProduct.Price = vegproduct.Price;
            existingProduct.Description = vegproduct.Description;
            existingProduct.StockQuantity = vegproduct.StockQuantity;
            existingProduct.IdCategory = vegproduct.IdCategory == 0 ? null : vegproduct.IdCategory;

            await context.SaveChangesAsync();
            return NoContent();
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
