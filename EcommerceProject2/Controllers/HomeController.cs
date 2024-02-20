using EcommerceProject2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EcommerceProject2.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var entities = new EcommerceDBEntities2();

            var productList = entities.Products
                    .SqlQuery("Select * from Products")
                    .ToList<Products>();

            TempData["products"] = productList;
            return View();
        }

        public ActionResult Details(int id)
        {
            var entities = new EcommerceDBEntities2();

            var query = "SELECT TOP 1 * FROM Products WHERE Product_id = @id";
            var parameters = new SqlParameter("@id", id);
            var result = entities.Database.SqlQuery<Products>(query, parameters).FirstOrDefault();

            // Retrieve product details
            var product = entities.Products
            .Include(p => p.Reviews.Select(r => r.Customer)) // Include customer information in reviews
            .Where(p => p.Product_id == id)
            .FirstOrDefault();


            if (product != null)
            {
                double averageRating = 0.0;
                if (product.Reviews != null && product.Reviews.Any())
                {
                    // Sum all ratings
                    int totalRatings = product.Reviews.Sum(r => Convert.ToInt32(r.Rating));

                    // Calculate average
                    averageRating = (double)totalRatings / product.Reviews.Count;
                }

                // Set the AverageRating property
               
                ViewBag.AverageRating = averageRating;
                TempData["AverageRating"] = averageRating;
                ViewBag.TotalReviews = product.Reviews.Count;

                // Check if the product's stock is sufficient
                if (result.stock < 10)
                {
                    ViewBag.WarningMessage = $"Warning: Stock for product '{result.Product_name}' is running low! There is left {result.stock}";
                }
                else
                {
                    ViewBag.StockMessage = $"Warning: Stock for product '{result.Product_name}' is {result.stock}!";
                }
            }


            return View(product);
        }

        //SEARCHING//
        public ActionResult SearchProducts(string search, int categoryId = 0, int minPrice = 0, int maxPrice = int.MaxValue)
        {
            using (var entities = new EcommerceDBEntities2())
            {
                var query = entities.Products.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Product_name.Contains(search) || p.Product_Desc.Contains(search));
                }

                if (categoryId > 0)
                {
                    query = query.Where(p => p.Category_id == categoryId);
                }

                query = query.Where(p => p.price >= minPrice && p.price <= maxPrice);

                var searchResults = query.ToList();

                // SearchResults view'ını kullanarak sonuçları gönder
                return View("SearchProducts", searchResults);
            }
        }

        public ActionResult Contact(int id)
        {
            
            var entities = new EcommerceDBEntities2();

                string query = "SELECT * FROM Customer WHERE Customer_id = @CustomerId";
                var parameters = new SqlParameter("@CustomerId", id);
                var info = entities.Database.SqlQuery<Customer>(query, parameters).FirstOrDefault();

                return View(info);
        }

        public ActionResult AdminContact(int id)
        {
            
            var entities = new EcommerceDBEntities2();

            string query = "SELECT * FROM Admin WHERE Admin_id = @adminID";
            var parameters = new SqlParameter("@adminID", id);
            var info = entities.Database.SqlQuery<Admin>(query, parameters).FirstOrDefault();

            return View(info);
        }

        
        public ActionResult Categories(int id)
        {
            var entities = new EcommerceDBEntities2();
            List<Products> productsList;
            switch (id)
            {
                case 14:
                    productsList = entities.Products
                                        .SqlQuery("SELECT * FROM Products WHERE Category_id = @id", new SqlParameter("@id", id))
                                        .ToList<Products>();
                    TempData["product"] = productsList;
                    break;
                case 15:
                    productsList = entities.Products
                                       .SqlQuery("SELECT * FROM Products WHERE Category_id = @id", new SqlParameter("@id", id))
                                       .ToList<Products>();
                    TempData["product"] = productsList;
                    break;
                case 16:
                    productsList = entities.Products
                                        .SqlQuery("SELECT * FROM Products WHERE Category_id = @id", new SqlParameter("@id", id))
                                        .ToList<Products>();
                    TempData["product"] = productsList;
                    break;
                case 17:
                    productsList = entities.Products
                                        .SqlQuery("SELECT * FROM Products WHERE Category_id = @id", new SqlParameter("@id", id))
                                        .ToList<Products>();
                    TempData["product"] = productsList;
                    break;
                case 18:
                    productsList = entities.Products
                                        .SqlQuery("SELECT * FROM Products WHERE Category_id = @id", new SqlParameter("@id", id))
                                        .ToList<Products>();
                    TempData["product"] = productsList;
                    break;
                case 19:
                    productsList = entities.Products
                                        .SqlQuery("SELECT * FROM Products WHERE Category_id = @id", new SqlParameter("@id", id))
                                        .ToList<Products>();
                    TempData["product"] = productsList;
                    break;

            }


            return View();
        }
    }
}