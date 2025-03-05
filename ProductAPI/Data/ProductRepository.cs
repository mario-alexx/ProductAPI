using ProductAPI.Models;

namespace ProductAPI.Data
{
    public class ProductRepository
    {
        private readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 1200.50m },
            new Product { Id = 2, Name = "Mouse", Price = 25.99m },
            new Product { Id = 3, Name = "Keyboard", Price = 45.75m }
        };

        public IEnumerable<Product> GetAll() => _products;
       
        public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);
       
        public void Add(Product product)
        {
            product.Id = _products.Max(p => p.Id) + 1;
            _products.Add(product);
        }

        public bool Update(int id, Product updatedProduct)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;

            return true;
        }

        public bool Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null) return false;

            _products.Remove(product);
            return true;
        }
    }
}
