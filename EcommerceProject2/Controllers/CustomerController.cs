using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EcommerceProject2.Models;

namespace EcommerceProject2.Controllers
{
    public class CustomerController : Controller
    {
        
        // GET: Customer
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost] 
        public ActionResult Login(Customer customer) 
        {
            
            
            var entities = new EcommerceDBEntities2();

            if (string.IsNullOrEmpty(customer.Customer_Email) || string.IsNullOrEmpty(customer.Password))
            {
                ViewBag.error = "Username and Password cannot be left blank!";
                return View();
            }

            var parameter1 = new SqlParameter("@Email", customer.Customer_Email);
            var parameter2 = new SqlParameter("@passw", customer.Password);
            var cs = entities.Customer.SqlQuery("SELECT * FROM Customer WHERE Customer_Email= @Email AND Password = @passw ", parameter1,parameter2).FirstOrDefault();

            
            if (cs != null)
            {
             
                FormsAuthentication.SetAuthCookie(customer.Customer_Email, false);
                Session["Customer_id"] = cs.Customer_id.ToString();
                Session["Customer_Email"] = cs.Customer_Email.ToString();
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.error = "Invalid Email or Password!";

            }
            return View();
        }
        [HttpGet]
        public ActionResult SignUp()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Customer cus)
        {
            using (var entities = new EcommerceDBEntities2()){

                if (string.IsNullOrEmpty(cus.Customer_Email) || string.IsNullOrEmpty(cus.Password) || string.IsNullOrEmpty(cus.First_name) || string.IsNullOrEmpty(cus.Last_name) || string.IsNullOrEmpty(cus.Address) || string.IsNullOrEmpty(cus.Phone))
                {
                    ViewBag.error = "Username and Password cannot be left blank!";
                    return View();
                }

                string su = "INSERT INTO Customer(Customer_Email,First_name,Last_name,Password,Address,Phone) VALUES(@Email,@name,@srname,@passw,@address,@phone)";

                int noOfRowInserted = entities.Database.ExecuteSqlCommand(su,
                    new SqlParameter("@Email", cus.Customer_Email),
                    new SqlParameter("@passw", cus.Password),
                    new SqlParameter("@name", cus.First_name),
                    new SqlParameter("@srname", cus.Last_name),
                    new SqlParameter("@address", cus.Address),
                    new SqlParameter("@phone", cus.Phone));


            }

            return RedirectToAction("Login");

            

        }
        [HttpGet]
        public ActionResult UpdateProfile(int id)
        {
            // Retrieve the customer information from the database using the current user's ID
            //int customerId = Convert.ToInt32(Session["Customer_id"]);

            var entities = new EcommerceDBEntities2();
            var customer = entities.Customer.Find(id);

            if (customer == null)
            {
                return HttpNotFound(); // Handle the case where the customer is not found
            }

            return View(customer);
        }
        [HttpPost]
        public ActionResult UpdateProfile(Customer updatedCustomer)
        {
            if (ModelState.IsValid)
            {
                var customerID = Convert.ToInt32(Session["Customer_id"]);

                if (customerID > 0)
                {
                    var entities = new EcommerceDBEntities2();
                    var existingCustomer = entities.Customer.Find(customerID);

                    if (existingCustomer != null)
                    {
                        // Update properties with new values
                        existingCustomer.Customer_Email = updatedCustomer.Customer_Email;
                        existingCustomer.First_name = updatedCustomer.First_name;
                        existingCustomer.Last_name = updatedCustomer.Last_name;
                        existingCustomer.Password = updatedCustomer.Password;
                        existingCustomer.Address = updatedCustomer.Address;
                        existingCustomer.Phone = updatedCustomer.Phone;

                        // Save changes to the database
                        entities.Entry(existingCustomer).State = EntityState.Modified;
                        entities.SaveChanges();

                        //TempData["SuccessMessage"] = "Profile updated successfully!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Customer not found");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Customer ID not valid");
                }
            }

            // If ModelState is not valid or customer is not found, return to the update profile page with errors
            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear(); // Oturum verilerini temizle
            Session.Abandon(); // Oturumu sonlandır
            return RedirectToAction("Login");
        }
    }
}