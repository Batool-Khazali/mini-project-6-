using MiniMerce.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace MiniMerce.Controllers
{
    public class HomeController : Controller
    {

        private MiniMerceEntities db = new MiniMerceEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Login_Signup()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult signup(user u, string confirmPwd)
        {
            if (ModelState.IsValid)
            {

                if (db.users.Any(x => x.email == u.email))
                {
                    ViewBag.Message = "email already used";
                }
                else if (u.password != confirmPwd)
                {
                    ViewBag.Message2 = "password do not match";
                }
                else if (db.users.Any(x => x.email != u.email) && u.password == confirmPwd)
                {
                    db.users.Add(u);
                    db.SaveChanges();
                    Response.Write("<script>alert('regestration successful')</script>");
                };
            }
            return RedirectToAction("Login_Signup");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(user u)
        {
            var query = db.users.SingleOrDefault(m => (m.email == u.email || m.name == u.email) && m.password == u.password);

            if (query != null)
            {

                HttpCookie authCookie = new HttpCookie("AuthCookie", u.email);
                authCookie.Expires = DateTime.Now.AddHours(1);
                Response.Cookies.Add(authCookie);

                Response.Write("<script>alert('Login successful')</script>");

                return RedirectToAction("Index");
            }
            else
            {

                Response.Write("<script>alert('invalid login')</script>");

                return RedirectToAction("Login_Signup");
            }

        }


        public ActionResult Logout()
        {
            if (Request.Cookies["AuthCookie"] != null)
            {
                var cookie = new HttpCookie("AuthCookie");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index");
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public ActionResult profile(user u)
        {
            HttpCookie authCookie = Request.Cookies["AuthCookie"];

            if (authCookie != null)
            {
                string userEmail = authCookie.Value;

                var userProfile = db.users.FirstOrDefault(x => x.email == userEmail);

                if (userProfile != null)
                {
                    return View(userProfile);
                }

            }
            return View();
        }


        public ActionResult editProfile(user u)
        {
            user NUEditGet = db.users.Find(u.ID);

            if (NUEditGet == null)
            {
                Response.Write("<script>alert('id not found in database')");
                return HttpNotFound();
            }

            return View(NUEditGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editProfile(user nu, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                user profileEditPost = db.users.Find(nu.ID);
                if (profileEditPost == null)
                {
                    return HttpNotFound("User not found");
                }

                if (upload != null && upload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(upload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/my images/"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/img/my images/")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/img/my images/"));
                    }


                    upload.SaveAs(path);
                    profileEditPost.image = fileName;
                }


                profileEditPost.name = nu.name;
                profileEditPost.email = nu.email;
                profileEditPost.phone = nu.phone;

                db.SaveChanges();
                return RedirectToAction("Profile");
            }

            return View(nu);
        }



        public ActionResult editpassword(int id)
        {
            user NUEditGet = db.users.Find(id);


            if (NUEditGet == null)
            {
                Response.Write("<script>alert('id not found in database')");
                return HttpNotFound();
            }


            return View(NUEditGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editpassword(user u, string oldPwd, string newPwd, string confirmPwd)
        {
            if (ModelState.IsValid)
            {
                user pwdEdit = db.users.Find(u.ID);

                if (pwdEdit.password != oldPwd)
                {
                    ViewBag.old = "wrong password";
                }
                else if (newPwd != confirmPwd)
                {
                    ViewBag.con = "password do not match";
                }
                else
                {
                    if (pwdEdit == null)
                    {
                        Response.Write("<script>alert('id not found in database')");
                        return HttpNotFound();
                    }

                    pwdEdit.password = newPwd;


                    db.SaveChanges();
                    return RedirectToAction("Profile");
                }
            }
            return View(u);
        }




        public ActionResult editaddress(int id)
        {
            user NUEditGet = db.users.Find(id);

            if (NUEditGet == null)
            {
                Response.Write("<script>alert('user not found in database')");
                return HttpNotFound();
            }

            return View(NUEditGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editaddress(user u)
        {
            if (ModelState.IsValid)
            {
                user profileEditPost = db.users.Find(u.ID);
                if (profileEditPost == null)
                {
                    return HttpNotFound("User not found");
                }


                profileEditPost.address_country = u.address_country;
                profileEditPost.address_city = u.address_city;
                profileEditPost.address_street = u.address_street;

                db.SaveChanges();
                return RedirectToAction("Profile");
            }

            return View(u);
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public ActionResult fabricTypes(string id)
        {
            return View(db.fabric_types.ToList());
        }


        public ActionResult typesList(string id)
        {
            var FT = db.fabric_types.ToList();

            List<string> fabricList = new List<string>();

            foreach (var f in FT)
            {
                int FTId = 0;

                if (f.type == id)
                {
                    FTId = f.ID;


                    foreach (var item in db.products)
                    {

                        if (item.type_id == FTId)
                        {
                            string newItem = $"{item.name}, {item.image}, {item.price}, {item.quantity}, {item.color}, {item.description}, {id}, {item.ID}";

                            fabricList.Add(newItem);
                        }
                    }
                    return View(fabricList.ToList()); ;
                }

            }
            return View(fabricList.ToList());
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public ActionResult shopLocation(string id)
        {
            return View(db.shops.ToList());
        }


        public ActionResult locationList(string id)
        {
            var shopsList = db.shops.ToList();
            var shopStock = db.stocks.ToList();
            var productsList = db.products.ToList();

            //List<string> shopsByLocation = new List<string>();
            List<string> productsByLocation = new List<string>();


            foreach (var shop in shopsList)
            {
                if (shop.location == id)
                {
                    //string addedShop = $"{shop.ID}, {shop.name}";

                    //shopsByLocation.Add(addedShop);

                    foreach (var stockItem in shopStock)
                    {

                        if (stockItem.ID == shop.ID)
                        {

                            foreach (var row in productsList)
                            {
                                string addedProduct = $"{row.name}, {row.image}, {row.price}, {row.quantity}, {row.color}, {row.description}, {id}, {row.ID}";

                                productsByLocation.Add(addedProduct);
                            }

                        }

                    }

                }
            }

            return View(productsByLocation.ToList());
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public ActionResult fabricColor(string id)
        {
            var products = db.products.ToList();

            List<string> uniqueColors = new List<string>();

            foreach (var product in products)
            {
                var colors = product.color.Split(';');

                foreach (var color in colors)
                {
                    string newColor = color.Trim();

                    if (!uniqueColors.Contains(newColor))
                    {
                        uniqueColors.Add(newColor);
                    }
                }
            }

            return View(uniqueColors);
        }


        public ActionResult colorList(string id)
        {
            var productsList = db.products.ToList();

            List<string> productsByColor = new List<string>();

            foreach (var product in productsList)
            {
                if (product.color.Contains(id))
                {
                    string newItem = $"{product.name}, {product.image}, {product.price}, {product.quantity}, {product.color}, {product.description}, {product.ID}";


                    productsByColor.Add(newItem);
                }
            }

            return View(productsByColor);
        }



        /* /////////////////////////////////////////////////////////////////////////////////////// */
        /* /////////////////////////////////////////////////////////////////////////////////////// */
        /* /////////////////////////////////////////////////////////////////////////////////////// */



        public ActionResult Details(int id)
        {
            product specificProduct = db.products.Find(id);

            return View(specificProduct);
        }



        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        public ActionResult addToCart(int id, int quantitySelected)
        {
            product product = db.products.Find(id);

            HttpCookie authCookie = Request.Cookies["AuthCookie"];
            int userId;

            if (authCookie != null)
            {
                string userEmail = authCookie.Value;
                var userProfile = db.users.FirstOrDefault(x => x.email == userEmail);
                userId = userProfile.ID;

                var userCart = db.carts.FirstOrDefault(x => x.user_id == userId);

                if (userCart == null)
                {
                    userCart = new cart
                    {
                        user_id = userId,
                        total = 0 
                    };
                    db.carts.Add(userCart);
                    db.SaveChanges(); 
                }

                float itemTotal = quantitySelected * (float)product.price;

                cart_items newItem = new cart_items
                {
                    cart_id = userCart.ID, 
                    product_id = id,
                    amount = quantitySelected,
                    item_total = itemTotal
                };

                db.cart_items.Add(newItem);

                userCart.total += (float)itemTotal;

                db.SaveChanges();
            }

            return RedirectToAction("Details", "Home", new { id });
        }


        public ActionResult cartPage()
        {
            // Retrieve the authenticated user's information
            HttpCookie authCookie = Request.Cookies["AuthCookie"];
            int userId;

            if (authCookie != null)
            {
                string userEmail = authCookie.Value;
                var userProfile = db.users.FirstOrDefault(x => x.email == userEmail);
                userId = userProfile.ID;

                // Get the user's cart
                var userCart = db.carts.FirstOrDefault(x => x.user_id == userId);

                if (userCart != null)
                {
                    // Retrieve the items in the cart
                    var cartItems = db.cart_items.Where(x => x.cart_id == userCart.ID).ToList();

                    // Pass the cart items and total amount to the view using ViewBag
                    ViewBag.CartItems = cartItems;
                    ViewBag.Total = userCart.total;

                    return View();
                }
            }

            // If no cart is found or user is not authenticated, redirect to a different page
            return RedirectToAction("Index", "Home");
        }












    }
}