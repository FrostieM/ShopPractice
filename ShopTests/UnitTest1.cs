using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplicationBook2.Controllers;
using WebApplicationBook2.Models;
using WebApplicationBook2.Models.ViewModels;
using Xunit;


namespace WebApplicationBook2Tests
{
    public class UnitTest1
    {
        [Fact]
        public void CanPaginate()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }.AsQueryable());

            var controller = new ProductController(mock.Object){PageSize = 3};
            var result = controller.List(null, 2).ViewData.Model as ProductListViewModel;
            var prodArray = result?.Products.ToArray();

            Assert.True(prodArray?.Length == 2);
            Assert.Equal("P4", prodArray[0].Name);
            Assert.Equal("P5", prodArray[1].Name);
        }
        
        [Fact]
        public void CanSendPaginationViewModel()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"}
            }.AsQueryable());

            var controller = new ProductController(mock.Object) {PageSize = 3};
            var result = controller.List(null, 2).ViewData.Model as ProductListViewModel;
            var pageInfo = result?.PagingInfo;
            Assert.Equal(2, pageInfo?.CurrentPage);
            Assert.Equal(3, pageInfo?.ItemsPerPage);
            Assert.Equal(5, pageInfo?.TotalItems);
            Assert.Equal(2, pageInfo?.TotalPages);
        }

        [Fact]
        public void CanFilterProducts()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            }.AsQueryable());
            
            var controller = new ProductController(mock.Object){PageSize = 3};
            var result = (controller.List("Cat2")
                .ViewData.Model as ProductListViewModel)?.Products.ToArray();
            
            Assert.Equal(2, result?.Length);
            Assert.True(result?[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [Fact]
        public void GenerateCategorySpecificProductCount()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"},
            }.AsQueryable());
            
            var target = new ProductController(mock.Object) { PageSize = 3};
            var getModel = new Func<ViewResult, ProductListViewModel>(result =>
                result?.ViewData?.Model as ProductListViewModel);

            var res1 = getModel(target.List("Cat1"))?.PagingInfo.TotalItems;
            var res2 = getModel(target.List("Cat2"))?.PagingInfo.TotalItems;
            var res3 = getModel(target.List("Cat3"))?.PagingInfo.TotalItems;
            var resAll = getModel(target.List(null))?.PagingInfo.TotalItems;
            
            Assert.Equal(2, res1);
            Assert.Equal(2, res2);
            Assert.Equal(1, res3);
            Assert.Equal(5, resAll);
        }
    }
}