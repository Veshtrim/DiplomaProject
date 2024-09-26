using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IProductService
    {
        IEnumerable<ProductViewModel> GetAllProducts();
        ProductViewModel GetProductById(int id);
        void CreateProduct(ProductViewModel model);
        void UpdateProduct(ProductViewModel model);
        void DeleteProduct(int id);
    }
}
