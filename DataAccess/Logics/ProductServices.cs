using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Logics
{
    public class ProductServices
    {
        public List<Product> GetProducts()
        {
            using (var context = new NorthwindContext())
            {
                var products = context.Products.Include(p => p.Category).ToList();
                return products;
            }
        }

        public Product GetProductById(int ProductId)
        {
            using (var context = new NorthwindContext())
            {
                var product = context.Products.FirstOrDefault(p => p.ProductId == ProductId);
                return product;
            }
        }

        public void AddProduct(Product product)
        {
            using (var context = new NorthwindContext())
            {
                context.Products.Add(product);
                context.SaveChanges();
            }
        }

        public void UpdateProduct(int ProductId, string ProductName, int? CategoryId, decimal? UnitPrice, bool Disconnected, string? QuantityPerUnit)
        {
            using (var context = new NorthwindContext())
            {
                var product = context.Products.Find(ProductId);
                if (product != null)
                {
                    product.ProductName = ProductName;
                    product.CategoryId = CategoryId;
                    product.UnitPrice = UnitPrice;
                    product.Discontinued = Disconnected;
                    product.QuantityPerUnit = QuantityPerUnit;
                    context.Products.Update(product);
                    context.SaveChanges();
                }
            }
        }

        public bool DeleteProduct(int ProductId)
        {
            bool check = false;
            OrderDetailsService orderDetailsService = new OrderDetailsService();
            using (var context = new NorthwindContext())
            {
                var product = context.Products.Find(ProductId);
                if (product != null && !orderDetailsService.CheckOrderDetails(ProductId))
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                    check = true;
                }
                else
                {
                    check = false;
                }
                return check;
            }
        }
    }
}
