using EcommerceProject2.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EcommerceProject2.Controllers
{
   
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var entities = new EcommerceDBEntities2();

            var categoryList = entities.Category
                    .SqlQuery("Select * from Category")
                    .ToList<Category>();


            
            TempData["categories"] = categoryList;
            
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {


            var entities = new EcommerceDBEntities2();

            if (string.IsNullOrEmpty(admin.Admin_name) || string.IsNullOrEmpty(admin.Admin_pssw))
            {
                ViewBag.error = "Username and Password cannot be left blank!";
                return View();
            }

            var parameter3 = new SqlParameter("@Name", admin.Admin_name);
            var parameter4 = new SqlParameter("@passw", admin.Admin_pssw);
            var ad = entities.Admin.SqlQuery("SELECT * FROM Admin WHERE Admin_name= @Name AND Admin_pssw = @passw ", parameter3, parameter4).FirstOrDefault();


            if (ad != null)
            {
                FormsAuthentication.SetAuthCookie(admin.Admin_name, false);
                Session["Admin_id"] = ad.Admin_id.ToString();
                Session["Admin_name"] = ad.Admin_name.ToString();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = "Invalid Email or Password!";

            }
            return View();
        }


        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear(); // Oturum verilerini temizle
            Session.Abandon(); // Oturumu sonlandır
            return RedirectToAction("Login");
        }


        public ActionResult Category()
        {
            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Category(Category cat, HttpPostedFileBase imgFile)
        {
            String path = uploadimage(imgFile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded";
            }
            else
            {
                using (var entities = new EcommerceDBEntities2())
                {
                    string category = "INSERT INTO Category(Category_name,Category_image,Category_Desc,Category_admin_id) VALUES(@catName,@path,@desc,@id)";

                    int noOfRowInserted = entities.Database.ExecuteSqlCommand(category,
                        new SqlParameter("@catName", cat.Category_name),
                         new SqlParameter("@desc", cat.Category_Desc),
                        new SqlParameter("@path", cat.Category_image = path),
                        new SqlParameter("@id", cat.Category_admin_id = Convert.ToInt32(Session["Admin_id"].ToString())));



                    

                }
            }
            return RedirectToAction("Index"); 
        }


        [HttpGet]
        public ActionResult DeleteCategory()
        {

            return View();
        }

        [HttpPost]
        public ActionResult DeleteCategory(int CategoryID)
        {
            using (EcommerceDBEntities2 entities = new EcommerceDBEntities2())
            {
                //var product = entities.Products.Find(productID);

                string deleteProductsQuery = "DELETE FROM Products WHERE Category_id = @CategoryID";
                SqlParameter categoryIDParam = new SqlParameter("@CategoryID", CategoryID);
                entities.Database.ExecuteSqlCommand(deleteProductsQuery, categoryIDParam);

                string deleteCategoryQuery = "DELETE FROM Category WHERE Category_id = @CategoryID";
                entities.Database.ExecuteSqlCommand(deleteCategoryQuery, categoryIDParam);

                return RedirectToAction("Index");
            }

        }

        public ActionResult Inventory()
        {
            var entities = new EcommerceDBEntities2();

            var InventoryList = entities.Products
                    .SqlQuery("SELECT * FROM Products")
                    .ToList<Products>();

            TempData["Inventory"] = InventoryList;

            return View();
        }


      public ActionResult CustomerInfo()
        {
            var entities = new EcommerceDBEntities2();

            var CustomerList = entities.Customer
                .SqlQuery("SELECT * FROM Customer")
                .ToList<Customer>();

            TempData["Customer"] = CustomerList;
            
            return View();
        }

        public ActionResult Coupons() 
        {
            var entities = new EcommerceDBEntities2();

            var couponList = entities.Coupons
                .SqlQuery("SELECT * FROM Coupons")
                .ToList<Coupons>();

            TempData["Coupon"] = couponList;
            return View();
        }
        public ActionResult ActivateCoupon(int couponId)
        {
            using (var entities = new EcommerceDBEntities2())
            {
                var coupon = entities.Coupons.Find(couponId);

                if (coupon != null)
                {
                    coupon.IsActive = true;
                    entities.SaveChanges();
                }

                // Yeniden yönlendirme veya başka bir işlem yapılabilir
                return RedirectToAction("Coupons");
            }
        }

        public ActionResult DeactivateCoupon(int couponId)
        {
            using (var entities = new EcommerceDBEntities2())
            {
                var coupon = entities.Coupons.Find(couponId);

                if (coupon != null)
                {
                    coupon.IsActive = false;
                    entities.SaveChanges();
                }

                // Yeniden yönlendirme veya başka bir işlem yapılabilir
                return RedirectToAction("Coupons");
            }
        }

        public ActionResult AddCoupons(Coupons coup)
        {
            using (var entities = new EcommerceDBEntities2())
            {
                string coupons = "INSERT INTO Coupons(Coupon_Code,Discount,IsActive,Admin_id) VALUES(@coupName,@disc,@isact,@id)";

                int noOfRowInserted = entities.Database.ExecuteSqlCommand(coupons,
                    new SqlParameter("@coupName", coup.Coupon_Code),
                    new SqlParameter("@disc", coup.Discount),
                    new SqlParameter("@isact", coup.IsActive),
                    new SqlParameter("@id", coup.Admin_id = Convert.ToInt32(Session["Admin_id"].ToString())));

                return RedirectToAction("Coupons");

            }
        }
        //UPLOAD IMAGE CODE//
        public string uploadimage(HttpPostedFileBase file)
        {
            Random r = new Random();

            string path = "-1";

            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        string filename = random + Path.GetFileName(file.FileName);
                        string serverPath = Server.MapPath("~/Content/Images/");
                        path = Path.Combine(serverPath, filename);
                        file.SaveAs(path);
                        path = "Content/Images/" + filename;
                    }
                    catch (Exception ex)
                    {
                        path = "-1";

                        throw;
                    }
                }

                else
                {
                    Response.Write("<script>alert('Only .JPG , .JPEG , .PNG Formats Are Allowed')</script>");
                }

            }

            else
            {
                Response.Write("<script>alert('Please Select a File')</script>");

                path = "-1";
            }

            return path;
        }

    }
}