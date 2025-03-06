using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductAPI.Data;
using ProductAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var products = await _context.Products.Where(p => !p.IsDeleted).ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            _logger.LogInformation("Get alls products");

            if (page <= 0 || pageSize <= 0)
                return BadRequest("Page and page size must be greather than zero.");

            var query = _context.Products.Where(p => !p.IsDeleted);

            var totalItems = query.Count();

            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new 
            {
                TotalItems = totalItems, Page = page,
                Size = pageSize, Data = products
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null || product.IsDeleted)
                return NotFound();

            product.IsDeleted = true;
            return NoContent();
        }

        [HttpGet("search")] 
        public async Task<ActionResult<IEnumerable<Product>>> Search([FromQuery] string? name, [FromQuery] decimal? minPrice)
        {
            IEnumerable<Product> products = await _context.Products.ToListAsync();

            if(!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if(minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            products = products.Where(p => 
                (string.IsNullOrEmpty(name)) || p.Name.Contains(name, StringComparison.OrdinalIgnoreCase) &&
                (!minPrice.HasValue || p.Price >= minPrice.Value));

            return Ok(products);
        }
    }
}
