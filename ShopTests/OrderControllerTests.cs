using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplicationBook2.Controllers;
using WebApplicationBook2.Models;
using Xunit;

namespace WebApplicationBook2Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void CannotCheckoutEmptyCart()
        {
            var mock = new Mock<IOrderRepository>();
            var cart = new Cart();
            var order = new Order();
            var target = new OrderController(mock.Object, cart);
            var result = target.Checkout(order) as ViewResult;
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            Assert.False(result?.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void CannotCheckoutInvalidShippingDetails()
        {
            var mock = new Mock<IOrderRepository>();
            var cart = new Cart();
            cart.AddItem(new Product(), 1);
            var target = new OrderController(mock.Object, cart);
            target.ModelState.AddModelError("error", "error");
            var result = target.Checkout(new Order()) as ViewResult;
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            
            Assert.True(string.IsNullOrEmpty(result?.ViewName));
            Assert.False(result?.ViewData.ModelState.IsValid);
        }
        
        [Fact]
        public void CanCheckoutAndSubmitOrder()
        {
            var mock = new Mock<IOrderRepository>();
            var cart = new Cart();
            cart.AddItem(new Product(), 1);
            var target = new OrderController(mock.Object, cart);
            var order = new Order
            {
                City = "City", Country = "Cntry", Line1 = "line1", Name = "Order", State = "State", Zip = "Zip"
            };
            var result = target.Checkout(order) as RedirectToActionResult;
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            
            Assert.Equal("Completed", result?.ActionName);
        }
        
        
    }
}