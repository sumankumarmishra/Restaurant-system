using Restugrp07.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Restugrp07.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        Db db = new Db();


        public ActionResult Index()
        {
            return Redirect("~/account/login");
        }


        // GET: account/Login
        [HttpGet]
        public ActionResult Login()
        {

            //confirm user is not logged in
            string username = User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                return Redirect("myprofile");

            }

            //return view
            return View();
        }

        // POST: account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check if user is valid
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.EmailAddress.Equals(model.Username) && x.Password.Equals(model.Password)))
                {
                    isValid = true;

                    var uss = db.Users.Where(x => x.EmailAddress == model.Username).FirstOrDefault();
                    int idd;
                   

                }


                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }

                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }
        // GET: account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            return Redirect("~/account/signout");

        }

        public ActionResult signout()
        {
            FormsAuthentication.SignOut();

            return Redirect("~/account/login");

        }


        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View();
        }


        // Post: account/create-account
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Error in some values";

                return Redirect("~/account/login");
            }
            //check if password match
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                TempData["Error"] = "Password do not match";

                return Redirect("~/account/login");
            }

            int idd;
            using (Db db = new Db())
            {
                //make sure username is unique
                if (db.Users.Any(x => x.EmailAddress.Equals(model.EmailAddress)))
                {
                    TempData["Error"] = "Please provide a valid email";

                    model.EmailAddress = "";
                    return Redirect("~/account/login");
                }


                UserRole userRolesDTO = new UserRole()
                {
                    RoleId = 2

                };

                db.UserRoles.Add(userRolesDTO);
                db.SaveChanges();

                int id = userRolesDTO.UserId;
                idd = id;


                //create userDTO
                User userDTO = new User()
                {
                    Id = id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Status = "NEW"
                };
                //Add The DTO
                db.Users.Add(userDTO);
                //Save
                db.SaveChanges();
                //Add to UserRolesDTO


            }
           

            //redirect  
            return Redirect("~/account/login");
        }


        [HttpGet]
        public ActionResult VerifyAddress(int id)
        {
            var veri = db.Verifies.Where(x => x.AccountId == id).FirstOrDefault();
            veri.Code = "";

            return View(veri);
        }


        [HttpPost]
        public ActionResult VerifyAddress(Verify ve, int id, string Code)
        {
            var veri = db.Verifies.Where(x => x.AccountId == id && x.Code == Code).FirstOrDefault();
            if (veri != null)
            {
                User user = db.Users.Find(id);
                user.Status = "VERIFIED";
                db.SaveChanges();
            }
            else
            {
                return Content("Details do not correspond");
            }
            TempData["Success Message"] = "Account Verified";
            return RedirectToAction("Login");
        }



        [Authorize]
        public ActionResult UserNavPartial()
        {

            //get the username
            string username = User.Identity.Name;
            //declare model
            UserNavPartialVM model;

            using (Db db = new Db())
            {

                //get the user
                User dto = db.Users.FirstOrDefault(x => x.EmailAddress == username);
                //build the model
                model = new UserNavPartialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }
            //return patrial view with  model
            return PartialView(model);
        }






        // GET: account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {

            //get username
            string username = User.Identity.Name;

            //declare model
            UserProfileVM model;

            using (Db db = new Db())
            {
                //get user
                User dto = db.Users.FirstOrDefault(x => x.EmailAddress == username);

                //build model
                model = new UserProfileVM(dto);

            }
            //return view with model
            return View("UserProfile", model);
        }



        // Post: account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            //check if password match if need be
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password Does not Match");
                    return View("UserProfile", model);
                }

            }
            using (Db db = new Db())
            {
                //get username
                string username = User.Identity.Name;
                //make sure username is unique
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.EmailAddress == username))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "Already Exist");
                    model.Username = "";
                    return View("UserProfile", model);
                }
                //Edit DTO
                User dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                //Save
                db.SaveChanges();

            }
            //Set Temp Message
            TempData["Success Message"] = "You have edit your profile";
            //redirect
            return Redirect("~/account/user-profile");

        }








        public ActionResult GetOrderAddress()
        {
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            ViewBag.Bal = "";
            //get user id
            Db db = new Db();



            var ad = db.Addresses.Where(x => x.Username == User.Identity.Name).FirstOrDefault();

            if (ad == null)
            {
                ViewBag.City = "";
                ViewBag.Address = "";
                ViewBag.Province = "";
                ViewBag.Zip = "";
                return View();
            }

            ViewBag.City = ad.City;
            ViewBag.Address = ad.Addres;
            ViewBag.Province = ad.Province;
            ViewBag.Zip = ad.Zip;




            return View();
        }



        [HttpPost]
        public ActionResult GetOrderAddress(Address model)
        {

            var ad = db.Addresses.Where(x => x.Username == User.Identity.Name).ToList();

            if (ad.Count() == 0)
            {
                Address add = new Address()
                {
                    Addres = model.Addres,
                    City = model.City,
                    Province = model.Province,
                    Zip = model.Zip,
                    Username = User.Identity.Name,
                };
                db.Addresses.Add(add);
                db.SaveChanges();
                return RedirectToAction("myprofile");
            }

            else
            {
                int adid = ad.FirstOrDefault().Id;

                Address addd = db.Addresses.Find(adid);
                addd.Addres = model.Addres;
                addd.City = model.City;
                addd.Province = model.Province;
                addd.Zip = model.Zip;
                db.SaveChanges();

                TempData["sucess"] = "Delivery addres updated";
                return RedirectToAction("MyProfile");

            }

        }







        // GET: account/user-profile
        [HttpGet]
        [Authorize]
        public ActionResult MyProfile()
        {

            //get username
            string username = User.Identity.Name;

            //declare model
            UserProfileVM model;

            using (Db db = new Db())
            {
                //get user
                User dto = db.Users.FirstOrDefault(x => x.EmailAddress == username);

                //build model
                model = new UserProfileVM(dto);

            }
            //return view with model
            return View(model);
        }



        // Post: account/user-profile
        [HttpPost]
        [Authorize]
        public ActionResult MyProfile(UserProfileVM model)
        {
            //check model state
            if (!ModelState.IsValid)
            {
                return View("myprofile", model);
            }

            //check if password match if need be
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password Does not Match");
                    return View(model);
                }

            }
            using (Db db = new Db())
            {
                //get username
                string username = User.Identity.Name;
                //make sure username is unique
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.EmailAddress == username))
                {
                    ModelState.AddModelError("", "Username" + model.Username + "Already Exist");
                    model.Username = "";
                    return View("myprofile", model);
                }
                //Edit DTO
                User dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAddress = model.EmailAddress;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                //Save
                db.SaveChanges();

            }
            //Set Temp Message
            TempData["Success Message"] = "You have edit your profile";
            //redirect
            return Redirect("~/account/myprofile");

        }




        [Authorize]

        public ActionResult Orders()
        {
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && ((x.Statusnum != 1 && x.Ordertypenum == 1) || (x.Ordertypenum == 2))).ToArray().Select(x => new OrderVM(x)).ToList();

                //loop through List Of OrderVM
                foreach (var order in orders)
                {
                    //Intialize Product Dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    //declare total
                    int total = 0;
                    //Intialize List Of OrderDetailsDTO
                    List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    //loop through List Of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsDTO)
                    {

                        string productName = "";


                        if (orderDetails.Product2Id == 0)
                        {

                            //Get The Product
                            Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                            //Get The Product Price
                            int price = product.Price;
                            //Get The Product Name
                            productName = product.Name;
                            //Add to Product dictionary
                            productsAndQty.Add(productName, orderDetails.Quantity);
                            //Get Total
                            total += price;
                        }


                        else
                        {
                            Partrequest rq = db.Partrequests.Find(orderDetails.Product2Id);


                            int price = rq.Price;
                            //Get The Product Name
                            productName = rq.Partnumber;
                            //Add to Product dictionary
                            productsAndQty.Add(productName, orderDetails.Quantity);
                            //Get Total
                            total += price;
                        }



                    }
                    //Add to ordersforuserVM List
                    ordersForUser.Add(new OrdersForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = order.TotalPrice,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt,
                        Destination = order.Destination,
                        Product2Id = orderDetailsDTO.FirstOrDefault().Product2Id,
                        DeliveryFee = order.DeliveryFee,
                        Tems = order.Tems,
                        Ordertypenum = order.Ordertypenum,
                        Counter = order.Counter,
                    });
                }

            }

            //Return View With List Of OrdersForUserVM

            return View(ordersForUser);
        }





        public ActionResult RequestRefund(int id)
        {
            Order ord = db.Orders.Find(id);
            return View(ord);
        }

        [HttpPost]
        public ActionResult RefundForm(string reason, int id)
        {
            Order ord = db.Orders.Find(id);
            ord.Statusnum = 10;
            ord.Status = "REFUND REQUREST FILLED";
            db.SaveChanges();

            Refund reff = new Refund()
            {
                CustomerEmail = User.Identity.Name,
                Date = DateTime.UtcNow,
                Destination = ord.Destination,
                OrderNum = id,
                statusnum = 1,
                PickupAddress = "461 boom street pietermaritzburg 3201",
                Reason = reason,

                Status = "WAITING FOR APPROVAL",

            };
            db.Refunds.Add(reff);
            db.SaveChanges();

            return RedirectToAction("MyRefunds");
        }





        public ActionResult MyRefunds()
        {
            var reff = db.Refunds.Where(x => x.CustomerEmail == User.Identity.Name).ToList();
            return View(reff);
        }




        public ActionResult Track(int id)
        {
            Order ord = db.Orders.Find(id);

            var del = db.Deliveries.Where(x => x.OrderId == ord.OrderId).FirstOrDefault();
            int did = 0;
            if (del != null)
            {
                did = del.Id;
                var dell = db.Deliveries.Find(did);
                ViewBag.D = dell.PickUpTime;

                return View(ord);
            }



            return View(ord);
        }
    }
}