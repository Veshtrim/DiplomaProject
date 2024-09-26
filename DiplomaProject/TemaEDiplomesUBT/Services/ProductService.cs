using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Services.IServices;

namespace TemaEDiplomesUBT.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ProductViewModel> GetAllProducts()
        {
            return _context.Products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category
            }).ToList();
        }

        public ProductViewModel GetProductById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return null;

            return new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category
            };
        }

        public void CreateProduct(ProductViewModel model)
        {
            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Category = model.Category
            };

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(ProductViewModel model)
        {
            var product = _context.Products.Find(model.Id);
            if (product == null) return;

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Category = model.Category;

            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return;

            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }

}
