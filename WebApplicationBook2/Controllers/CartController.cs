using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplicationBook2.Infrastructure;
using WebApplicationBook2.Models;
using WebApplicationBook2.Models.ViewModels;

namespace WebApplicationBook2.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly Cart _cart;
        
        public CartController(IProductRepository repository, Cart cartService)
        {
            _repository = repository;
            _cart = cartService;
        }

        [Route("Cart/Index")]
        public ViewResult Index(string returnUrl) => 
            View(new CartIndexViewModel 
            {
                Cart = _cart,
                ReturnUrl = returnUrl
            });

        [Route("Cart/AddToCart")]
        public RedirectToActionResult AddToCart(int productId, string returnUrl)
        {
            var product = _repository.Products
                .FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                _cart.AddItem(product, 1);
            }
            return RedirectToAction("Index", new {returnUrl});
        }

        [Route("Cart/RemoveFromCart")]
        public RedirectToActionResult RemoveFromCart(int productId, string returnUrl)
        {
            var product = _repository.Products
                .FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                _cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new {returnUrl});
        }
    }
}