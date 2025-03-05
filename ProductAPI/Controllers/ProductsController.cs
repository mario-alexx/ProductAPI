using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<Product>> GetAll()
        //{
        //    return Ok(_repository.GetAll());
        //}

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            _logger.LogInformation("Get alls products");
            var products = _repository.GetAll()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetById(int id)
        {
            var product = _repository.GetById(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public ActionResult<Product> Create(Product product)
        {
            _repository.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, Product product)
        {
            if (!_repository.Update(id, product)) 
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (!_repository.Delete(id))
                return NotFound();

            return NoContent();
        }

        [HttpGet("search")] 
        public ActionResult<IEnumerable<Product>> Search([FromQuery] string? name, [FromQuery] decimal? minPrice)
        {
            var products = _repository.GetAll();

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
