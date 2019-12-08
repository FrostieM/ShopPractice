using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Moq;
using WebApplicationBook2.Components;
using WebApplicationBook2.Models;
using Xunit;

namespace WebApplicationBook2Tests
{
    public class NavigationMenuViewComponentTests
    {
        [Fact]
        public void CanSelectCategories()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                new Product {ProductID = 4, Name = "P4", Category = "Oranges"}
            }.AsQueryable());

            var target = new NavigationMenuViewComponent(mock.Object);
            var results = ((IEnumerable<string>) ((ViewViewComponentResult) target.Invoke())
                .ViewData.Model).ToArray();
            
            Assert.True(new[]
            {
                "Apples", "Oranges", "Plums"
            }.SequenceEqual(results));
        }

        [Fact]
        public void IndicatesSelectedCategory()
        {
            const string categoryToSelect = "Apples";
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"}
            }.AsQueryable());

            var target = new NavigationMenuViewComponent(mock.Object)
            {
                ViewComponentContext = new ViewComponentContext
                {
                    ViewContext = new ViewContext {RouteData = new RouteData()}
                }
            };

            target.RouteData.Values["category"] = categoryToSelect;
            var result = (string) (target.Invoke() as ViewViewComponentResult)?.ViewData["SelectedCategory"];
            
            Assert.Equal(categoryToSelect, result);
        }
    }
}