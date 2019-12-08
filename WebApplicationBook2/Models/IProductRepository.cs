using System.Linq;

namespace WebApplicationBook2.Models
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
    }
}