using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebApplicationBook2.Models;

namespace WebApplicationBook2.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _repository;
        private readonly Cart _cart;

        public OrderController(IOrderRepository repository, Cart cartService)
        {
            _repository = repository;
            _cart = cartService;
        }

        public ViewResult List() => View(_repository.Orders.Where(o => !o.Shipped));

        [HttpPost]
        public IActionResult MarkShipped(int orderID)
        {
            var order = _repository.Orders
                .FirstOrDefault(o => o.OrderID == orderID);
            if (order != null)
            {
                order.Shipped = true;
                _repository.SaveOrder(order);
            }

            return RedirectToAction(nameof(List));
        }

        [Route("Order/Checkout")]
        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        [ActionName("CheckoutForm")]
        [Route("Order/ActionForm")]
        public IActionResult Checkout(Order order)
        {
            if (!_cart.Lines.Any())
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }

            if (!ModelState.IsValid) return View("Checkout", order);
            
            order.Lines = _cart.Lines.ToArray();
            _repository.SaveOrder(order);
            return RedirectToAction("Completed");
        }

        [Route("Order/Completed")]
        public ViewResult Completed()
        {
            _cart.Clear();
            return View();
        }
    }
}