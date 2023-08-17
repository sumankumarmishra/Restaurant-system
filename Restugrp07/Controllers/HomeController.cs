using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using Restugrp07.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Image = iTextSharp.text.Image;

namespace Restugrp07.Controllers
{
    public class HomeController : Controller
    {
        Db db = new Db();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Categories()
        {
            var cat = db.Categories.ToList();
            return View(cat);
        }


        public ActionResult categorty(int catId)
        {
            var pr = db.Products.Where(x => x.CategoryId == catId).ToList();
            return View(pr);
        }

        public ActionResult Search(string q)
        {

            var pro = db.Products.Where(x => x.Name.Contains(q));

            return View(pro.ToList());
        }


        public ActionResult Products()
        {
            var pr = db.Products.ToList();
            return View(pr);
        }

        public ActionResult Productdetails(string qname)
        {
            Product pr = db.Products.Where(x => x.Query == qname).FirstOrDefault();
            return View(pr);
        }


        public ActionResult ProductsSlide()
        {

            var pr = db.Products.ToList().Take(20);

            return View(pr);
        }


        public ActionResult Relatedproducts(string qname)
        {
            var pr = db.Products.Where(x => x.Query == qname).FirstOrDefault();

            var prlist = db.Products.Where(x => x.CategoryName == pr.CategoryName).ToList().Take(15);

            return View(prlist);
        }




        public ActionResult Minicart()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "cart is empty.";

                return View();
            }
            // Calculate total and save to ViewBag

            double total = 0;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            int productQuantity = 0;
            int status = 0;
            using (Db db = new Db())

                foreach (var item in cart)
                {

                    //Get Current User Balance
                    var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    productQuantity += p.Quantity;

                    if (productQuantity < item.Quantity)
                    {
                        status = 1;
                    }
                    //Get Current User Balance
                }
            ViewBag.Stat = status;

            ViewBag.Quant = productQuantity;


            ViewBag.GrandTotal = total;
            ViewBag.Dilivloyal = total - 100;
            ViewBag.Categories = new SelectList(db.PickPoints.ToList(), "Id", "PointAddress");
            // Return view with list
            return View(cart);
        }



        public ActionResult Cart()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "cart is empty.";

                return View();
            }
            // Calculate total and save to ViewBag

            double total = 0;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            int productQuantity = 0;
            int status = 0;
            using (Db db = new Db())

                foreach (var item in cart)
                {

                    //Get Current User Balance
                    var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    productQuantity += p.Quantity;

                    if (productQuantity < item.Quantity)
                    {
                        status = 1;
                    }
                    //Get Current User Balance
                }
            ViewBag.Stat = status;

            ViewBag.Quant = productQuantity;


            ViewBag.GrandTotal = total;
            ViewBag.Dilivloyal = total - 100;
            ViewBag.Categories = new SelectList(db.PickPoints.ToList(), "Id", "PointAddress");
            // Return view with list
            return View(cart);
        }




        public ActionResult AddtoCart(int productId)
        {

            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            int qty = 1;

            Db cc = new Db();


            // Init CartVM
            CartVM model = new CartVM();


            using (Db db = new Db())
            {
                // Get the product
                Product product = db.Products.Find(productId);
                int pr = product.Price;

                var productInCart = cart.FirstOrDefault(x => x.ProductId == productId);

                // If not, add new



                if ((productInCart == null) && (product.Quantity >= 1))
                {


                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = pr,
                        Image = product.ImageName,
                        ProductQuant = product.Quantity
                    });

                }


                else if ((productInCart != null) && (product.Quantity - 1 >= productInCart.Quantity))
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }

                {
                    Redirect("/home/cart");
                }

                string dat = DateTime.UtcNow.DayOfWeek.ToString().ToUpper();


            }

            // Get total qty and price and add to model
            int price = 0;

            foreach (var item in cart)
            {
                qty += item.Quantity;

                price += item.Quantity * item.Price;
            }


            model.Quantity = qty;

            model.Price = price;


            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return Redirect("/home/cart");
        }



        public ActionResult Getqoutation()
        {



            return View();
        }


        public ActionResult IncrementProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                Product product = db.Products.Find(productId);

                // Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                if (product.Quantity > model.Quantity)
                {
                    model.Quantity++;
                    // Store needed data
                    var result = new { qty = model.Quantity, price = model.Price };
                    // Return json with data
                    return RedirectToAction("cart");
                }

                else
                {
                    return RedirectToAction("cart");

                }
                // Increment qty

            }

        }

        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                // Get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };
                // Return json
                return RedirectToAction("cart");
            }
        }

        // GET: /Cart/RemoveProduct
        public ActionResult RemoveProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                // Remove model from list
                cart.Remove(model);
            }

            return RedirectToAction("cart");

        }



        public ActionResult Clearcart()
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            var ck = cart.ToList();

            foreach (var item in ck)
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == item.ProductId);
                // Remove model from list
                cart.Remove(model);
            }


            return RedirectToAction("cart");

        }

        public ActionResult Reorder(int id)
        {
            var details = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            foreach (var item in details)
            {
                Product pr = db.Products.Find(item.ProductId);

                cart.Add(new CartVM()
                {
                    Color = item.Color,
                    Image = pr.ImageName,
                    Price = pr.Price,
                    ProductId = pr.Id,
                    Quantity = item.Quantity,
                    ProductName = pr.Name,
                    ProductQuant = item.Quantity,
                });
            }
            Session["cart"] = cart;



            return Redirect("/home/cart");
        }


        [Authorize]
        public ActionResult GetQuote(CartVM model)
        {



            var incart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            GetQuery invoname = new GetQuery();
            string invon = invoname.Main().ToString();

            System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/") + invon + ".pdf", FileMode.Create);



            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);

            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

            pdfDoc.Open();
            try
            {

                try
                {

                    Attachment datat = new Attachment(Server.MapPath("~/New/img/logo-2.png"));

                    datat.Dispose();

                }

                catch
                {
                    return Redirect("/Invoice/UploadCompanylogo");
                }

                //Top Heading
                Chunk chunk = new Chunk(DateTime.UtcNow.Date.ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                pdfDoc.Add(chunk);
                //Horizontal Line


                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);


                //Table
                PdfPTable table = new PdfPTable(5)
                {
                    WidthPercentage = 100,
                    //0=Left, 1=Centre, 2=Right
                    HorizontalAlignment = 0,
                    SpacingBefore = 20f,
                    SpacingAfter = 30f
                };
                ////////
                ///


                //Cell no 1
                PdfPCell cell = new PdfPCell
                {
                    Border = 0
                };
                Image image = Image.GetInstance(Server.MapPath("~/New/img/logo-2.png"));
                image.ScaleAbsolute(100, 100);
                cell.AddElement(image);
                table.AddCell(cell);



                chunk = new Chunk("FOOD LOVERS" + "\n" + "Restugrp07@gmail.com" + "\n" + "Durban North, South Africa" + "\n" + "durban" + "\n" + "Kwazulunatal" + "\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));



                cell = new PdfPCell
                {
                    Border = 0
                };
                var para3 = new Paragraph(chunk)
                {
                    Alignment = Element.ALIGN_LEFT
                };
                para3.Alignment = -100;

                cell.AddElement(para3);
                table.AddCell(cell);




                chunk = new Chunk("", FontFactory.GetFont("Helvetica Neue", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell
                {
                    Border = 0
                };

                var para4 = new Paragraph(chunk)
                {
                    Alignment = Element.ALIGN_LEFT
                };
                para4.Alignment = -100;

                cell.AddElement(para4);
                table.AddCell(cell);



                chunk = new Chunk("", FontFactory.GetFont("Helvetica Neue", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell
                {
                    Border = 0
                };

                var para5 = new Paragraph(chunk)
                {
                    Alignment = Element.ALIGN_LEFT
                };
                para5.Alignment = -100;

                cell.AddElement(para5);
                table.AddCell(cell);




                table.AddCell(cell);
                chunk = new Chunk("Order Number:" + "Partial Order" + "\n" + "Date:" + DateTime.Now + "\n", FontFactory.GetFont("Helvetica Neue", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell
                {
                    Border = 0
                };

                table.AddCell(cell);




                //Add table to document
                pdfDoc.Add(table);

                //Horizontal Line
                //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                //pdfDoc.Add(line) ;

                //Table
                table = new PdfPTable(5)
                {
                    WidthPercentage = 100,
                    HorizontalAlignment = 0,
                    SpacingBefore = 20f,
                    SpacingAfter = -0f
                };

                //Cell


                var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

                if (cart != null)
                {
                    cell = new PdfPCell();
                    chunk = new Chunk("Items", FontFactory.GetFont("Helvetica Neue", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell.Colspan = 5;
                    var para13 = new Paragraph(chunk)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };


                    cell.AddElement(para13);
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    table.AddCell("Item Number");
                    table.AddCell("Item " + Environment.NewLine);
                    table.AddCell("Price" + Environment.NewLine);
                    table.AddCell("Total" + Environment.NewLine);
                    table.AddCell("Quantity" + Environment.NewLine);
                    pdfDoc.Add(table);

                    table = new PdfPTable(5)
                    {
                        WidthPercentage = 100,
                        HorizontalAlignment = 0,
                        SpacingBefore = 0f,
                        SpacingAfter = 30f
                    };
                }



                foreach (var item in cart)
                {


                    line = new Paragraph(new Chunk(item.ProductId.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                    line = new Paragraph(new Chunk(item.ProductName.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                    line = new Paragraph(new Chunk(item.Price.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);
                    line = new Paragraph(new Chunk("R:" + item.Total.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line); line = new Paragraph(new Chunk(item.Quantity.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                    table.AddCell(line);


                }


                pdfDoc.Add(table);

                int tot = 0;

                foreach (var item in incart)
                {
                    tot += item.Total;

                }

                Chunk chunk5 = new Chunk("Grand Total:\n", FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                pdfDoc.Add(chunk5);

                Chunk chunk6 = new Chunk(tot + "\n", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                pdfDoc.Add(chunk6);

                foreach (var item in incart)
                {
                    //if (3>2)
                    //{
                    //    chunk = new Chunk("Click Me", FontFactory.GetFont("Helvetica Neue", 10, iTextSharp.text.Font.BOLD, BaseColor.BLUE));
                    //    chunk.SetAnchor("https://www.google.com");
                    //    chunk.SetBackground(BaseColor.RED);
                    //    pdfDoc.Add(chunk);
                    //}
                    //else
                    //{

                    //}


                }




                line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                pdfDoc.Add(line);

                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                pdfDoc.CloseDocument();
                fs.Close();


                string sender = "21705746@dut4life.ac.za";
                string password = "Dut981217";




                string recipient = User.Identity.Name;
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com")
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,

                };
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, password);
                client.EnableSsl = true;
                client.Credentials = credentials;
                Attachment data = new Attachment(Server.MapPath("~/Images/" + invon + ".pdf"));


                var mail = new MailMessage(sender.Trim(), recipient.Trim())
                {
                    Subject = "Att: " + "Quotation",
                    Priority = MailPriority.High,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess,
                    Body = "Good Day  " + User.Identity.Name + "," + "\n" + " Please find your order quotation attached here" + "\n" + "" + "\n" + "" + "\n" + "" + "\n" + ""
                };
                mail.Attachments.Add(data);
                client.Send(mail);

                TempData["Success"] = "Quotation sent to your email!!";

                Session["indata"] = null;
                Session["incart"] = null;
                Session["cart"] = null;


                return RedirectToAction("Cart");
            }

            catch (Exception ex)
            {

                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                TempData["Error"] = "Failed To Send Quotation!!";
                //throw ex;

                return RedirectToAction("Cart");
            }

        }





        public ActionResult DeliveryFee(int id, string dist)
        {


            double distt = double.Parse(dist.Replace(" ", "").Replace("km", "").Replace(".", ","));


            double delfee = distt * 1;

            Order order = db.Orders.Find(id);

            order.TotalPrice = order.TotalPrice + int.Parse(delfee.ToString().Replace(",", "")) / 10;
            order.DeliveryFee = int.Parse(delfee.ToString().Replace(",", "")) / 10;
            db.SaveChanges();

            return RedirectToAction("PaymentOptions");
        }










        // POST: /Cart/PlaceOrder
        [Authorize]
        [HttpPost]
        public ActionResult PlaceOrder(string shipping)
        {
            // Get cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Get username
            string username = User.Identity.Name;
            int orderId = 0;

            using (Db db = new Db())
            {
                Order orderDTO = new Order();

                var add = db.Addresses.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
                if (add == null)
                {
                    TempData["Error"] = "Please add delivery address";
                    return Redirect("/Account/myprofile");
                }





                // Get user id
                var q = db.Users.FirstOrDefault(x => x.EmailAddress == username);
                int userId = q.Id;
                int tot = 0;

                foreach (var item in cart)
                {

                    tot = cart.Sum(x => x.Total);
                }

                orderDTO.TotalPrice = tot;
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.UtcNow.AddHours(2);
                orderDTO.Statusnum = 1;
                orderDTO.Ordertypenum = 1;
                orderDTO.Ordertype = "STRAIGHT";

                orderDTO.Status = "NEW";
                string deliveryaddress = "";
                if (shipping == "address")
                {
                    deliveryaddress = add.Addres + " " + add.City + " " + add.Province + " " + add.Zip;
                }

                if (shipping == "TOBECOLLECTED")
                {
                    deliveryaddress = "TOBECOLEECTED";
                }
                if (shipping == "non")
                {

                    TempData["Error"] = "Please choose delivery option";
                    return RedirectToAction("Cart");
                }


                orderDTO.Destination = deliveryaddress;

                db.Orders.Add(orderDTO);
                db.SaveChanges();
                orderId = orderDTO.OrderId;


                OrderDetails orderDetailsDTO = new OrderDetails();
                // Add to OrderDetailsDTO
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Color = item.Color;
                    orderDetailsDTO.Quantity = item.Quantity;
                    orderDetailsDTO.Producttypenum = 1;
                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();
                }
            }

            // Reset session
            Session["cart"] = null;

            return RedirectToAction("PaymentOptions");
        }


        public ActionResult Cartcount()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            if (cart != null)
            {
                int c = 0;

                foreach (var item in cart)
                {
                    c = c + item.Quantity;
                }

                return Content(c.ToString());
            }
            else
            {
                return Content("0");
            }

        }



        [Authorize]
        public ActionResult PaymentOptions()
        {
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            ViewBag.Bal = "";

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Statusnum == 1).ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                string dest = orders.FirstOrDefault().Destination;




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

                    });
                }

            }

            //Return View With List Of OrdersForUserVM

            return View(ordersForUser);
        }


        /// <summary>
        /// ///////////////////////////////////////////////////
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        /// 







     
        [Authorize]
        public ActionResult DeductQuantity(Nullable<int> orderid)
        {
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM

                if (orderid != null)
                {
                    List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.OrderId == orderid).ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                    //loop through List Of OrderVMx.
                    foreach (var order in orders)
                    {
                        //Intialize Product Dictionary
                        Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                        //declare total
                        int total = 0;
                        //Intialize List Of OrderDetailsDTO
                        List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                        if (orderDetailsDTO.FirstOrDefault().Product2Id == 0)
                        {



                            //loop through List Of OrderDetailsDTO
                            foreach (var orderDetails in orderDetailsDTO)
                            {

                                //Get The Product
                                Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                                //Get The Product Price
                                int price = product.Price;
                                //Get The Product Name
                                string productName = product.Name;
                                //Add to Product dictionary
                                productsAndQty.Add(productName, orderDetails.Quantity);
                                //Get Total
                                total += orderDetails.Quantity * price;

                            }



                            /////////////////////////////////////
                            ////////////////////////////////////
                            ///
                            foreach (var item in orderDetailsDTO)
                            {

                                Partrequest rq = db.Partrequests.Find(item.Product2Id);


                                //Get Current User Balance
                                var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                                int productQuantity = p.Quantity;
                                //Get Current User Balance


                                var productId = item.ProductId; // Set a user ID that you would like to retrieve

                                var dbContext = new Db(); // Your entity framework DbContext

                                // Retrieve a user from the database
                                var product = dbContext.Set<Product>().Find(productId);

                                // Update a property on your user
                                product.Quantity = productQuantity - item.Quantity;

                                // Save the new value to the database
                                dbContext.SaveChanges();


                            }


                            foreach (var item in orderDetailsDTO)
                            {

                                //Get Current User Balance
                                var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                                string productQuantity = p.Status;
                                //Get Current User Balance

                                var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                                var dbContext = new Db(); // Your entity framework DbContext

                                // Retrieve a user from the database
                                var orderr = dbContext.Set<Order>().Find(orderId);

                                // Update a property on your user
                                orderr.Status = "PROCESSING";
                                orderr.Statusnum = 2;

                                GetQuery query = new GetQuery();

                                orderr.OrderCode = query.Main();

                                // Save the new value to the database
                                dbContext.SaveChanges();
                            }


                            //Add to ordersforuserVM List
                            ordersForUser.Add(new OrdersForUserVM()
                            {
                                OrderNumber = order.OrderId,
                                Total = total,
                                ProductsAndQty = productsAndQty,
                                CreatedAt = order.CreatedAt

                            });

                        }

                        else
                        {


                            //loop through List Of OrderDetailsDTO
                            foreach (var orderDetails in orderDetailsDTO)
                            {

                                Partrequest rq = db.Partrequests.Find(orderDetails.Product2Id);

                                //Get The Product
                                Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                                //Get The Product Price
                                int price = rq.Price;
                                //Get The Product Name
                                string productName = rq.Partnumber;
                                //Add to Product dictionary
                                productsAndQty.Add(productName, orderDetails.Quantity);
                                //Get Total
                                total += price;

                            }



                            /////////////////////////////////////
                            ////////////////////////////////////
                            ///
                            foreach (var item in orderDetailsDTO)
                            {


                                Partrequest rq = db.Partrequests.Find(item.Product2Id);

                                rq.Status = "PAYMENT RECEIVED";
                                rq.Statusnum = 5;
                                db.SaveChanges();


                            }


                            foreach (var item in orderDetailsDTO)
                            {

                                //Get Current User Balance
                                var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                                string productQuantity = p.Status;
                                //Get Current User Balance

                                var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                                var dbContext = new Db(); // Your entity framework DbContext

                                // Retrieve a user from the database
                                var orderr = dbContext.Set<Order>().Find(orderId);

                                // Update a property on your user
                                orderr.Status = "PROCESSING";
                                orderr.Statusnum = 2;

                                GetQuery query = new GetQuery();

                                orderr.OrderCode = query.Main();

                                // Save the new value to the database
                                dbContext.SaveChanges();
                            }


                            //Add to ordersforuserVM List
                            ordersForUser.Add(new OrdersForUserVM()
                            {
                                OrderNumber = order.OrderId,
                                Total = total,
                                ProductsAndQty = productsAndQty,
                                CreatedAt = order.CreatedAt

                            });


                        }


                    }

                }


                else
                {

                    List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Statusnum == 1).ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                    //loop through List Of OrderVMx.
                    foreach (var order in orders)
                    {
                        //Intialize Product Dictionary
                        Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                        //declare total
                        int total = 0;
                        //Intialize List Of OrderDetailsDTO
                        List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                        if (orderDetailsDTO.FirstOrDefault().Product2Id == 0)
                        {



                            //loop through List Of OrderDetailsDTO
                            foreach (var orderDetails in orderDetailsDTO)
                            {

                                //Get The Product
                                Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                                //Get The Product Price
                                int price = product.Price;
                                //Get The Product Name
                                string productName = product.Name;
                                //Add to Product dictionary
                                productsAndQty.Add(productName, orderDetails.Quantity);
                                //Get Total
                                total += orderDetails.Quantity * price;

                            }



                            /////////////////////////////////////
                            ////////////////////////////////////
                            ///
                            foreach (var item in orderDetailsDTO)
                            {

                                Partrequest rq = db.Partrequests.Find(item.Product2Id);


                                //Get Current User Balance
                                var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                                int productQuantity = p.Quantity;
                                //Get Current User Balance


                                var productId = item.ProductId; // Set a user ID that you would like to retrieve

                                var dbContext = new Db(); // Your entity framework DbContext

                                // Retrieve a user from the database
                                var product = dbContext.Set<Product>().Find(productId);

                                // Update a property on your user
                                product.Quantity = productQuantity - item.Quantity;

                                // Save the new value to the database
                                dbContext.SaveChanges();


                            }


                            foreach (var item in orderDetailsDTO)
                            {

                                //Get Current User Balance
                                var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                                string productQuantity = p.Status;
                                //Get Current User Balance

                                var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                                var dbContext = new Db(); // Your entity framework DbContext

                                // Retrieve a user from the database
                                var orderr = dbContext.Set<Order>().Find(orderId);

                                // Update a property on your user
                                orderr.Status = "PROCESSING";
                                orderr.Statusnum = 2;

                                GetQuery query = new GetQuery();

                                orderr.OrderCode = query.Main();

                                // Save the new value to the database
                                dbContext.SaveChanges();
                            }


                            //Add to ordersforuserVM List
                            ordersForUser.Add(new OrdersForUserVM()
                            {
                                OrderNumber = order.OrderId,
                                Total = total,
                                ProductsAndQty = productsAndQty,
                                CreatedAt = order.CreatedAt

                            });

                        }

                        else
                        {


                            //loop through List Of OrderDetailsDTO
                            foreach (var orderDetails in orderDetailsDTO)
                            {

                                Partrequest rq = db.Partrequests.Find(orderDetails.Product2Id);

                                //Get The Product
                                Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                                //Get The Product Price
                                int price = rq.Price;
                                //Get The Product Name
                                string productName = rq.Partnumber;
                                //Add to Product dictionary
                                productsAndQty.Add(productName, orderDetails.Quantity);
                                //Get Total
                                total += price;

                            }



                            /////////////////////////////////////
                            ////////////////////////////////////
                            ///
                            foreach (var item in orderDetailsDTO)
                            {


                                Partrequest rq = db.Partrequests.Find(item.Product2Id);

                                rq.Status = "PAYMENT RECEIVED";
                                rq.Statusnum = 5;
                                db.SaveChanges();


                            }


                            foreach (var item in orderDetailsDTO)
                            {

                                //Get Current User Balance
                                var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                                string productQuantity = p.Status;
                                //Get Current User Balance

                                var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                                var dbContext = new Db(); // Your entity framework DbContext

                                // Retrieve a user from the database
                                var orderr = dbContext.Set<Order>().Find(orderId);

                                // Update a property on your user
                                orderr.Status = "PROCESSING";
                                orderr.Statusnum = 2;

                                GetQuery query = new GetQuery();

                                orderr.OrderCode = query.Main();

                                // Save the new value to the database
                                dbContext.SaveChanges();
                            }


                            //Add to ordersforuserVM List
                            ordersForUser.Add(new OrdersForUserVM()
                            {
                                OrderNumber = order.OrderId,
                                Total = total,
                                ProductsAndQty = productsAndQty,
                                CreatedAt = order.CreatedAt

                            });


                        }


                    }

                }







            }




            if (orderid != null)
            {
                return Redirect("/home/ActionQrCode?orderiD=" + orderid);
            }


            //Return View With List Of OrdersForUserVM

            return RedirectToAction("ActionQrCode");
        }



        [Authorize]

        public ActionResult ActionQrCode(Nullable<int> orderiD)
        {

            int wid = 0;
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM


                if (orderiD != null)
                {
                    List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.OrderId == orderiD).ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                    //loop through List Of OrderVMx.
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

                            //Get The Product
                            Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                            //Get The Product Price
                            int price = product.Price;
                            //Get The Product Name
                            string productName = product.Name;
                            //Add to Product dictionary
                            productsAndQty.Add(productName, orderDetails.Quantity);
                            //Get Total
                            total += orderDetails.Quantity * price;

                            var dbContext = new Db(); // Your entity framework DbContext

                            // Retrieve a user from the database



                        }



                        /////////////////////////////////////
                        ////////////////////////////////////
                        ///
                        foreach (var item in orderDetailsDTO)
                        {
                            //Get Current User Balance
                            var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                            int productQuantity = p.Quantity;
                            //Get Current User Balance


                            var productId = item.ProductId; // Set a user ID that you would like to retrieve

                            var dbContext = new Db(); // Your entity framework DbContext

                            // Retrieve a user from the database
                            var product = dbContext.Set<Product>().Find(productId);

                            // Update a property on your user

                            // Save the new value to the database
                            dbContext.SaveChanges();
                        }
                        foreach (var item in orderDetailsDTO)
                        {

                            //Get Current User Balance
                            var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                            string productQuantity = p.Status;
                            //Get Current User Balance




                            var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                            var dbContext = new Db(); // Your entity framework DbContext

                            // Retrieve a user from the database
                            var orderr = dbContext.Set<Order>().Find(orderId);

                            if (3 > 2)
                            {
                                wid = orderId;
                            }


                        }
                        int orderid = orders.FirstOrDefault().OrderId;
                        /// qr code generator
                        Order ordd = db.Orders.Find(orderid);

                        string Message = ordd.OrderCode;


                        QRCodeGenerator ObjQr = new QRCodeGenerator();

                        QRCodeData qrCodeData = ObjQr.CreateQrCode(Message, QRCodeGenerator.ECCLevel.Q);

                        Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                        using (MemoryStream ms = new MemoryStream())

                        {

                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                            byte[] byteImage = ms.ToArray();

                            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Verify", Server.MapPath(@"\")));
                            string pathString = Path.Combine(originalDirectory.ToString(), "\\");

                            if (!Directory.Exists(pathString))
                            {
                                Directory.CreateDirectory(pathString);

                            }

                            ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                            bitMap.Save(Server.MapPath("~/images/Verify/" + User.Identity.Name + wid + "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
                        }


                        GetQuery invoname = new GetQuery();
                        string invon = invoname.Main().ToString();
                        System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/") + invon + ".pdf", FileMode.Create);

                        Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

                        pdfDoc.Open();


                        try
                        {



                            Order odd = db.Orders.Find(wid);
                            odd.OrderCode = invon;
                            db.SaveChanges();

                            //Top Heading
                            Chunk chunk = new Chunk(DateTime.UtcNow.AddHours(2).ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                            pdfDoc.Add(chunk);

                            //Horizontal Line
                            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            pdfDoc.Add(line);


                            //Table
                            PdfPTable table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            //0=Left, 1=Centre, 2=Right
                            table.HorizontalAlignment = 0;
                            table.SpacingBefore = 20f;
                            table.SpacingAfter = 30f;
                            ////////
                            ///






                            //Cell no 1
                            PdfPCell cell = new PdfPCell();
                            cell.Border = 0;
                            //Image image = Image.GetInstance(Server.MapPath("~/images/Verify/" + User.Identity.Name + wid + "qrcode.png"));
                            //image.ScaleAbsolute(100, 100);
                            //cell.AddElement(image);
                            table.AddCell(cell);




                            chunk = new Chunk("ORDER NUM: " + wid + "\nDATE: \n" + odd.CreatedAt + "\nADDRESS: " + odd.Destination + "\nDELIVERY FEE: R " + odd.DeliveryFee + "\nBALANCE DUE :R 0\nTOTAL: R " + odd.TotalPrice, FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell();
                            cell.Border = 0;
                            var para3 = new Paragraph(chunk);
                            para3.Alignment = Element.ALIGN_LEFT;
                            para3.Alignment = -100;

                            cell.AddElement(para3);
                            table.AddCell(cell);




                            chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell();
                            cell.Border = 0;

                            var para4 = new Paragraph(chunk);
                            para4.Alignment = Element.ALIGN_LEFT;
                            para4.Alignment = -100;

                            cell.AddElement(para4);
                            table.AddCell(cell);



                            chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell();
                            cell.Border = 0;

                            var para5 = new Paragraph(chunk);
                            para5.Alignment = Element.ALIGN_LEFT;
                            para5.Alignment = -100;

                            cell.AddElement(para5);
                            table.AddCell(cell);


                            chunk = new Chunk("FOOD LOVERS" + "\n" + "Restugrp07@gmail.com" + "\n" + "Durban North, South Africa" + "\n" + "durban" + "\n" + "Kwazulunatal" + "\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));

                            //Cell no 2
                            cell = new PdfPCell();
                            cell.Border = 0;

                            var para1 = new Paragraph(chunk);
                            para1.Alignment = Element.ALIGN_RIGHT;


                            cell.AddElement(para1);
                            table.AddCell(cell);



                            //Add table to document
                            pdfDoc.Add(table);

                            //Horizontal Line
                            //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            //pdfDoc.Add(line);

                            //Table
                            table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            table.HorizontalAlignment = 0;
                            table.SpacingBefore = 20f;
                            table.SpacingAfter = -0f;

                            //Cell
                            cell = new PdfPCell();
                            chunk = new Chunk("ORDER ITEMS", FontFactory.GetFont("Daytona Condensed Light", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                            cell.Colspan = 5;
                            var para13 = new Paragraph(chunk);
                            para13.Alignment = Element.ALIGN_CENTER;


                            cell.AddElement(para13);
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);

                            table.AddCell("PRUDUCT NUMBER");
                            table.AddCell("TITLE" + Environment.NewLine);
                            table.AddCell("PRICE" + Environment.NewLine);
                            table.AddCell("QUANTITY" + Environment.NewLine);
                            table.AddCell("TOTAL" + Environment.NewLine);
                            pdfDoc.Add(table);





                            var cart = db.OrderDetails.Where(x => x.OrderId == wid);

                            table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            table.HorizontalAlignment = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 30f;




                            foreach (var item in cart)
                            {
                                line = new Paragraph(new Chunk(item.ProductId.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);
                                line = new Paragraph(new Chunk(item.Product.Name.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);

                                line = new Paragraph(new Chunk("R:" + item.Product.Price.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);
                                line = new Paragraph(new Chunk(item.Quantity.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);

                                line = new Paragraph(new Chunk("R:" + (item.Quantity * item.Product.Price).ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);


                            }


                            pdfDoc.Add(table);



                            Paragraph para = new Paragraph();
                            para.Add("THANK YOU");
                            pdfDoc.Add(para);

                            //Horizontal Line
                            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            pdfDoc.Add(line);

                            pdfWriter.CloseStream = false;
                            pdfDoc.Close();
                            pdfDoc.CloseDocument();
                            fs.Close();


                            return Redirect("/Account/myprofile");
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }


                }


                else
                {



                    List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Statusnum == 2).ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                    //loop through List Of OrderVMx.
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

                            //Get The Product
                            Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                            //Get The Product Price
                            int price = product.Price;
                            //Get The Product Name
                            string productName = product.Name;
                            //Add to Product dictionary
                            productsAndQty.Add(productName, orderDetails.Quantity);
                            //Get Total
                            total += orderDetails.Quantity * price;

                            var dbContext = new Db(); // Your entity framework DbContext

                            // Retrieve a user from the database



                        }



                        /////////////////////////////////////
                        ////////////////////////////////////
                        ///
                        foreach (var item in orderDetailsDTO)
                        {
                            //Get Current User Balance
                            var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                            int productQuantity = p.Quantity;
                            //Get Current User Balance


                            var productId = item.ProductId; // Set a user ID that you would like to retrieve

                            var dbContext = new Db(); // Your entity framework DbContext

                            // Retrieve a user from the database
                            var product = dbContext.Set<Product>().Find(productId);

                            // Update a property on your user

                            // Save the new value to the database
                            dbContext.SaveChanges();
                        }
                        foreach (var item in orderDetailsDTO)
                        {

                            //Get Current User Balance
                            var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                            string productQuantity = p.Status;
                            //Get Current User Balance




                            var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                            var dbContext = new Db(); // Your entity framework DbContext

                            // Retrieve a user from the database
                            var orderr = dbContext.Set<Order>().Find(orderId);

                            if (3 > 2)
                            {
                                wid = orderId;
                            }


                        }
                        int orderid = orders.FirstOrDefault().OrderId;
                        /// qr code generator
                        Order ordd = db.Orders.Find(orderid);

                        string Message = ordd.OrderCode;


                        QRCodeGenerator ObjQr = new QRCodeGenerator();

                        QRCodeData qrCodeData = ObjQr.CreateQrCode(Message, QRCodeGenerator.ECCLevel.Q);

                        Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                        using (MemoryStream ms = new MemoryStream())

                        {

                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                            byte[] byteImage = ms.ToArray();

                            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Verify", Server.MapPath(@"\")));
                            string pathString = Path.Combine(originalDirectory.ToString(), "\\");

                            if (!Directory.Exists(pathString))
                            {
                                Directory.CreateDirectory(pathString);

                            }

                            ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                            bitMap.Save(Server.MapPath("~/images/Verify/" + User.Identity.Name + wid + "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
                        }


                        GetQuery invoname = new GetQuery();
                        string invon = invoname.Main().ToString();
                        System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/") + invon + ".pdf", FileMode.Create);

                        Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

                        pdfDoc.Open();


                        try
                        {



                            Order odd = db.Orders.Find(wid);
                            odd.OrderCode = invon;
                            db.SaveChanges();

                            //Top Heading
                            Chunk chunk = new Chunk(DateTime.UtcNow.AddHours(2).ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                            pdfDoc.Add(chunk);

                            //Horizontal Line
                            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            pdfDoc.Add(line);


                            //Table
                            PdfPTable table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            //0=Left, 1=Centre, 2=Right
                            table.HorizontalAlignment = 0;
                            table.SpacingBefore = 20f;
                            table.SpacingAfter = 30f;
                            ////////
                            ///






                            //Cell no 1
                            PdfPCell cell = new PdfPCell();
                            cell.Border = 0;
                            //Image image = Image.GetInstance(Server.MapPath("~/images/Verify/" + User.Identity.Name + wid + "qrcode.png"));
                            //image.ScaleAbsolute(100, 100);
                            //cell.AddElement(image);
                            table.AddCell(cell);




                            chunk = new Chunk("ORDER NUM: " + wid + "\nDATE: \n" + odd.CreatedAt + "\nADDRESS: " + odd.Destination + "\nDELIVERY FEE: R " + odd.DeliveryFee + "\nBALANCE DUE :R 0\nTOTAL: R " + odd.TotalPrice, FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell();
                            cell.Border = 0;
                            var para3 = new Paragraph(chunk);
                            para3.Alignment = Element.ALIGN_LEFT;
                            para3.Alignment = -100;

                            cell.AddElement(para3);
                            table.AddCell(cell);




                            chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell();
                            cell.Border = 0;

                            var para4 = new Paragraph(chunk);
                            para4.Alignment = Element.ALIGN_LEFT;
                            para4.Alignment = -100;

                            cell.AddElement(para4);
                            table.AddCell(cell);



                            chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell();
                            cell.Border = 0;

                            var para5 = new Paragraph(chunk);
                            para5.Alignment = Element.ALIGN_LEFT;
                            para5.Alignment = -100;
                            cell.AddElement(para5);
                            table.AddCell(cell);


                            chunk = new Chunk("FOOD LOVERS" + "\n" + "Restugrp07@gmail.com" + "\n" + "Durban North, South Africa" + "\n" + "durban" + "\n" + "Kwazulunatal" + "\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));

                            //Cell no 2
                            cell = new PdfPCell();
                            cell.Border = 0;

                            var para1 = new Paragraph(chunk);
                            para1.Alignment = Element.ALIGN_RIGHT;


                            cell.AddElement(para1);
                            table.AddCell(cell);



                            //Add table to document
                            pdfDoc.Add(table);

                            //Horizontal Line
                            //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            //pdfDoc.Add(line);

                            //Table
                            table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            table.HorizontalAlignment = 0;
                            table.SpacingBefore = 20f;
                            table.SpacingAfter = -0f;

                            //Cell
                            cell = new PdfPCell();
                            chunk = new Chunk("ORDER ITEMS", FontFactory.GetFont("Daytona Condensed Light", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                            cell.Colspan = 5;
                            var para13 = new Paragraph(chunk);
                            para13.Alignment = Element.ALIGN_CENTER;


                            cell.AddElement(para13);
                            cell.BackgroundColor = BaseColor.WHITE;
                            table.AddCell(cell);

                            table.AddCell("PRODUCT NUMBER");
                            table.AddCell("TITLE" + Environment.NewLine);
                            table.AddCell("PRICE" + Environment.NewLine);
                            table.AddCell("QUANTITY" + Environment.NewLine);
                            table.AddCell("TOTAL" + Environment.NewLine);
                            pdfDoc.Add(table);



                            var cart = db.OrderDetails.Where(x => x.OrderId == wid);

                            table = new PdfPTable(5);
                            table.WidthPercentage = 100;
                            table.HorizontalAlignment = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 30f;




                            foreach (var item in cart)
                            {
                                line = new Paragraph(new Chunk(item.ProductId.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);
                                line = new Paragraph(new Chunk(item.Product.Name.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);

                                line = new Paragraph(new Chunk("R:" + item.Product.Price.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);
                                line = new Paragraph(new Chunk(item.Quantity.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);

                                line = new Paragraph(new Chunk("R:" + (item.Quantity * item.Product.Price).ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                                table.AddCell(line);


                            }


                            pdfDoc.Add(table);



                            Paragraph para = new Paragraph();
                            para.Add("THANK YOU");
                            pdfDoc.Add(para);

                            //Horizontal Line
                            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                            pdfDoc.Add(line);

                            pdfWriter.CloseStream = false;
                            pdfDoc.Close();
                            pdfDoc.CloseDocument();
                            fs.Close();


                            return Redirect("/Account/myprofile");
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                    }



                }



            }

            return Redirect("/Account/myprofile");

        }






        ///Semester 2
        ///
        [Authorize, HttpGet]
        public ActionResult Requestpart()
        {
            return View();
        }




        [HttpPost, Authorize]
        public ActionResult Requestpart(Partrequest model, HttpPostedFileBase file)
        {

            string filename = "";

            if (file.ContentLength != null && file.InputStream != null)
            {
                string ext = file.ContentType.ToLower();
                //verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {


                        return View(model);


                    }
                }
                filename = file.FileName;

            }

            Partrequest pr = new Partrequest()
            {
                Useremail = User.Identity.Name,
                Condition = model.Condition,
                Date = DateTime.Now,
                Partcarmodel = model.Partcarmodel,
                Partimage = filename,
                Partnumber = model.Partnumber,
                Price = 0,
                Status = "REQUEST SUBMITED",
                Statusnum = 1,
            };

            db.Partrequests.Add(pr);
            db.SaveChanges();


            int id = pr.Id;





            //create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Parts");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Parts\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Parts\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Parts\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Parts\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            //check if a file was  uploaded
            if (file != null && file.ContentLength > 0)
            {
                //get file extension
                string ext = file.ContentType.ToLower();
                //verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {


                        return View(model);


                    }
                }
                //initilize image name
                string imageName = file.FileName;

                //set orignial and thumb image path
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                //save original image
                file.SaveAs(path);
                //create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            return Redirect("/home/myrequests");

        }


        [Authorize]
        public ActionResult Myrequests()
        {
            var rq = db.Partrequests.Where(x => x.Useremail == User.Identity.Name).ToList();

            return View(rq);
        }


        public ActionResult Requestdetails(int id)
        {
            Partrequest rq = db.Partrequests.Find(id);
            return View(rq);
        }


        public ActionResult Requestdeliveryoptions(int id)
        {
            Partrequest rq = db.Partrequests.Find(id);

            User user = db.Users.FirstOrDefault(x => x.EmailAddress == User.Identity.Name);

            Order ord = new Order()
            {
                CreatedAt = DateTime.Now,
                DeliveryFee = 0,
                Destination = "TOBECOLLECTED",
                OrderCode = "",
                Status = "NEW",
                Statusnum = 1,
                TotalPrice = rq.Price,
                UserId = user.Id,
                User = user,
            };
            db.Orders.Add(ord);
            db.SaveChanges();

            Partrequest rqq = db.Partrequests.Find(id);
            rqq.Orderid = ord.OrderId;
            db.SaveChanges();

            Product pr = db.Products.FirstOrDefault();

            OrderDetails det = new OrderDetails()
            {
                OrderId = ord.OrderId,
                ProductId = pr.Id,
                Producttypenum = 2,
                Quantity = 1,
                Product2Id = rq.Id,

            };

            db.OrderDetails.Add(det);
            db.SaveChanges();

            return Redirect("/home/getdeliveryadd?id=" + rq.Id);
        }




        public ActionResult Getdeliveryadd(int id)
        {
            Partrequest rq = db.Partrequests.Find(id);
            return View(rq);
        }



        public ActionResult Savedeliveryadd(int id)
        {
            var add = db.Addresses.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
            if (add == null)
            {
                TempData["Error"] = "Please add delivery address";
                return Redirect("/Account/myprofile");
            }

            else
            {


                string deliveryaddress = add.Addres + " " + add.City + " " + add.Province + " " + add.Zip;


                Partrequest rq = db.Partrequests.Find(id);

                Order ord = db.Orders.Find(rq.Orderid);
                ord.Destination = deliveryaddress;
                db.SaveChanges();

                return Redirect("/home/paymentoptions");
            }
        }



        public ActionResult Updaterequestpaymentstatus(int? id)
        {
            Partrequest rq = db.Partrequests.Find(id);


            rq.Status = "Payment received";
            rq.Statusnum = 5;
            db.SaveChanges();

            Order ord = new Order()
            {
                CreatedAt = DateTime.Now,

            };

            return View();
        }

        [Authorize]
        public ActionResult Tradeinrequest()
        {
            //Intialize Model
            ProductVM model = new ProductVM();
            //Add Select List Of Categories to Model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            //Return view With Model
            return View(model);
        }

        [Authorize, HttpPost]
        public ActionResult Tradeinrequest(ProductVM model, HttpPostedFileBase file)
        {
            GetQuery query = new GetQuery();
            //check model state
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {

                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }

            }
            //make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    TempData["Error"] = "Item already exist";
                    ModelState.AddModelError("", "Spare alredy exists!");
                    return View(model);
                }

            }
            //declare product id
            int id;
            //initilize and save productDTO
            using (Db db = new Db())
            {
                Product product = new Product();
                product.Name = model.Name;
                product.Query = model.Name.Replace(" ", "").Replace("&", "") + query.Main();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.Quantity = model.Quantity;



                Category catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                //Get the id
                id = product.Id;
            }

            //set tempdata messsage
            TempData["Success"] = "item added successfuly!";

            #region Upload Image

            //create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            //check if a file was  uploaded
            if (file != null && file.ContentLength > 0)
            {
                //get file extension
                string ext = file.ContentType.ToLower();
                //verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {

                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Wrong Image Extension !");
                        return View(model);


                    }
                }
                //initilize image name
                string imageName = file.FileName;
                //save mage name to DTO
                using (Db db = new Db())
                {
                    Product dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                //set orignial and thumb image path
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                //save original image
                file.SaveAs(path);
                //create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            #endregion

            //Redirect
            return RedirectToAction("AddProduct");
        }


        [HttpGet, Authorize]
        public ActionResult Tradein()
        {
            return View();
        }

        [HttpPost, Authorize]
        public ActionResult Tradein(Tradein model)
        {
            Tradein tr = new Tradein()
            {
                Assessoremail = "",
                Idcopy = "",
                Itemdescription = model.Itemdescription,
                Itemimage = "",
                Itemname = model.Itemname,
                Proofofownership = "",
                Purchasedate = model.Purchasedate,
                Statusnum = 1,
                Status = "WAITING FOR DOCUMENTS",
                Total = 0,
                Travelfee = 0,
                Useremail = User.Identity.Name,
                Requestassessor = model.Requestassessor,
            };

            db.Tradeins.Add(tr);
            db.SaveChanges();


            if (model.Requestassessor == true)
            {
                return Redirect("/home/AddAddress?appid=" + tr.Id);

            }

            else
            {
                return Redirect("/home/uploadtradeindocs?appid=" + tr.Id);

            }

        }


        [HttpGet]
        public ActionResult AddAddress(int appid)
        {
            Tradein tr = db.Tradeins.Find(appid);

            return View(tr);
        }


        public ActionResult AddAddress(Tradein model)
        {

            Tradein tr = db.Tradeins.Find(model.Id);
            tr.Address = model.Address;
            db.SaveChanges();


            return Redirect("/home/uploadtradeindocs?appid=" + tr.Id);
        }


        [HttpGet]
        public ActionResult Uploadtradeindocs(int appid)
        {

            Tradein tr = db.Tradeins.Find(appid);

            return View(tr);
        }


        [HttpPost]
        public ActionResult Uploadtradeindocs(Tradein model, HttpPostedFileBase proofofowership, HttpPostedFileBase idcopy, HttpPostedFileBase itemimage)
        {

            Tradein tr = db.Tradeins.Find(model.Id);

            tr.Proofofownership = proofofowership.FileName;
            tr.Idcopy = idcopy.FileName;
            tr.Itemimage = itemimage.FileName;
            tr.Status = "DOCUMENTS RECEIVED WAITING FOR APPROVAL";
            db.SaveChanges();

            int id = tr.Id;

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Docs");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString() + "\\Ownership");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString() + "\\Idcopy");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Docs\\" + id.ToString() + "\\Itemimage");

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            //set orignial and thumb image path
            var path = string.Format("{0}\\{1}", pathString3, proofofowership.FileName);
            var path2 = string.Format("{0}\\{1}", pathString4, idcopy.FileName);
            var path3 = string.Format("{0}\\{1}", pathString5, itemimage.FileName);

            //save original image
            proofofowership.SaveAs(path);
            idcopy.SaveAs(path2);

            itemimage.SaveAs(path3);

            return Redirect("/home/tradeins");
        }

        public ActionResult Tradeins()
        {

            var tr = db.Tradeins.Where(x => x.Useremail == User.Identity.Name).ToList();
            return View(tr);
        }


        public ActionResult tradeindetails(int appid)
        {
            Tradein tr = db.Tradeins.Find(appid);

            return View(tr);
        }

        [HttpGet, Authorize]
        public ActionResult Approveoffer(int appid)
        {
            Tradein tr = db.Tradeins.Find(appid);

            return View(tr);
        }

        [Authorize, HttpPost]
        public ActionResult Approveoffer(Tradein model, string signature, int appid)
        {
            if (signature != null)
            {
                Tradein tr = db.Tradeins.Find(appid);
                tr.Status = "CUSTOMER APPROVED";
                tr.customersignature = signature;
                tr.Statusnum = 4;
                tr.assessoersignature = signature;
                db.SaveChanges();


                User us = db.Users.FirstOrDefault(x => x.EmailAddress == User.Identity.Name);

                us.Credit = us.Credit + tr.Total;
                db.SaveChanges();



                return RedirectToAction("tradeins");
            }
            return View();
        }



        public ActionResult laybuy(int orderid)
        {

            Order ord = db.Orders.Find(orderid);
            ord.Ordertype = "LAYBUY";
            ord.Ordertypenum = 2;
            ord.Due = ord.TotalPrice;
            db.SaveChanges();

            return Redirect("/home/Laybuyoption?orderid=" + orderid);
        }


        public ActionResult Laybuyoption(int orderid)
        {
            Order ord = db.Orders.Find(orderid);

            return View(ord);
        }


        public ActionResult Laybuypaymentoptions(int orderid, int option)
        {

            int m = 0;

            if (option == 1)
            {
                m = 3;
            }
            if (option == 2)
            {
                m = 6;
            }

            Order ord = db.Orders.Find(orderid);

            ord.Expierdate = DateTime.Now.AddMonths(m);
            ord.Due = (ord.TotalPrice + ord.DeliveryFee) / m;
            ord.tems = m;

            db.SaveChanges();

            return View(ord);
        }


        public ActionResult laybuyrepayments(int orderid)
        {
            Order ord = db.Orders.Find(orderid);

            return View(ord);
        }


        public ActionResult laybuypayed(int orderid)
        {
            Order ord = db.Orders.Find(orderid);



            ord.couter = ord.couter + 1;
            ord.Statusnum = 1;
            db.SaveChanges();


            if (ord.tems == ord.couter)
            {

                Order ordd = db.Orders.Find(orderid);

                ordd.Statusnum = 2;

                ordd.Status = "PAYMENT RECEIVED";
                db.SaveChanges();

                return Redirect("/home/DeductQuantity");
            }

            return Redirect("/account/Myprofile");
        }


        public ActionResult Credit()
        {
            int cre = 0;

            if (Request.IsAuthenticated)
            {
                var cr = db.Users.FirstOrDefault(x => x.EmailAddress == User.Identity.Name);
                cre = cr.Credit;
            }
            else
            {
                cre = 0;
            }


            return Content(cre.ToString());
        }


        public ActionResult viewslip(int id)
        {
            Order o = db.Orders.Find(id);

            ViewBag.S = o.OrderCode + ".pdf";

            return View();
        }


        [HttpGet,Authorize]
        public ActionResult Reserveatable()
        {
            return View();
        }

        [HttpPost,Authorize]
        public ActionResult Reserveatable(Resevation model)
        {


            if( model.Numberofguests < 1|| model.Numberofguests >10)
            {
                ModelState.AddModelError("", "Please enter the number of guests between 1 - 10");

                return View(model);
            }

            Resevation res = new Resevation()
            {
                Numberofguests = model.Numberofguests,
                Occationdate = model.Occationdate,
                Date = DateTime.Now,
                Resevationtype = model.Resevationtype,
                Status = "INCOMPLETE",
                Statusnum = 1,
                Useremail = User.Identity.Name
            };
            db.Resevations.Add(res);
            db.SaveChanges();

            return Redirect("/home/resevationlayout?resid="+res.Id);
        }



        public ActionResult resevationlayout(int resId)
        {
            Resevation res = db.Resevations.Find(resId);

            return View(res);
        }

        [HttpPost]
        public ActionResult resevationlayout(Resevation model)
        {

            Resevation res = db.Resevations.Find(model.Id);
            res.Tablelayout = model.Tablelayout;
            res.Themecolor = model.Themecolor;
            db.SaveChanges();

            return Redirect("/home/Selectexclusivemeal?resId="+res.Id);
        }


        public ActionResult Selectexclusivemeal(int resId)
        {
            ViewBag.resId = resId;
            var m = db.Products.ToList();
            return View(m);
        }


        public ActionResult Addtoresevationmeal(int mealId, int resId)
        {

            Resevation res = db.Resevations.Find(resId);

            var rem = db.Resevationmeals.FirstOrDefault(x => x.ResevationId == resId && x.MealId == mealId);

            if(rem!= null)
            {
                Resevationmeal remm = rem;
                remm.Quantityordered = remm.Quantityordered + 1;
                db.SaveChanges();
            }

            else
            {
                Resevationmeal resm = new Resevationmeal()
                {
                    ResevationId = resId,
                    MealId = mealId,
                    Quantityordered = res.Numberofguests,
                };

                db.Resevationmeals.Add(resm);
                db.SaveChanges();
            }

           

            return Redirect("/home/Resevationmeals?resId=" + resId );
        }


        public ActionResult Resevationmeals(int resId)
        {
            ViewBag.resId = resId;

            var resm = db.Resevationmeals.Where(x => x.ResevationId == resId).ToList();

            return View(resm);
        }

        public ActionResult incre(int resmealId)
        {

            Resevationmeal resm = db.Resevationmeals.Find(resmealId);
            resm.Quantityordered = resm.Quantityordered + 1;
            db.SaveChanges();

            return Redirect("/home/resevationmeals?resId=" + resm.ResevationId);

        }

        public ActionResult rem(int resmealId)
        {

            Resevationmeal resm = db.Resevationmeals.Find(resmealId);
            db.Resevationmeals.Remove(resm);
            db.SaveChanges();

            return Redirect("/home/resevationmeals?resId=" + resm.ResevationId);

        }

        public ActionResult decre(int resmealId)
        {

            Resevationmeal resm = db.Resevationmeals.Find(resmealId);
            if(resm.Quantityordered < 2)
            {
                db.Resevationmeals.Remove(resm);
                db.SaveChanges();
            }
            else
            {
                resm.Quantityordered = resm.Quantityordered - 1;
                db.SaveChanges();
            }

            return Redirect("/home/resevationmeals?resId=" + resm.ResevationId);

        }


        public ActionResult Orderdetailsandpayment(int resId)
        {


            var resm = db.Resevationmeals.Where(x => x.ResevationId == resId).ToList();

            Resevation res = db.Resevations.Find(resId);
            int resfee = 100;

            if(res.Resevationtype == "VIP")
            {
                resfee = 150;
            }
            else if(res.Resevationtype == "VVIP")
            {
                resfee = 400;
            }
           
            
            if(res.Statusnum == 1)
            {
                res.Total = resm.Sum(x => x.Product.Price * x.Quantityordered) + resfee;
                res.Servicefee = resfee;
                res.Statusnum = 2;
                res.Status = "WAITING FOR PAYMENT";
                db.SaveChanges();
            }


            return View(res);
        }


        public ActionResult paymentresevationmeals(int resId)
        {
            var resm = db.Resevationmeals.Where(x => x.ResevationId == resId).ToList();

            return View(resm);
        }



        public ActionResult Resevationpaid(int resId)
        {
          
            

            using (Db db = new Db())
            {


                GetQuery code = new GetQuery();

                string c = code.Main();

                Resevation res = db.Resevations.Find(resId);
                res.Resevationcode = c;
                res.Statusnum = 3;
                res.Status = "PAIMENT RECEIVED";
                res.Resavationslip = c + ".pdf";
                db.SaveChanges();

                var originalDirectory1 = new DirectoryInfo(string.Format("{0}Images\\Resevation", Server.MapPath(@"\")));
                var pathString11 = Path.Combine(originalDirectory1.ToString());

                if (!Directory.Exists(pathString11))
                {
                    Directory.CreateDirectory(pathString11);
                }

                string Message = res.Resevationcode;


                QRCodeGenerator ObjQr = new QRCodeGenerator();

                QRCodeData qrCodeData = ObjQr.CreateQrCode(Message, QRCodeGenerator.ECCLevel.Q);

                Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                using (MemoryStream ms = new MemoryStream())

                {

                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    byte[] byteImage = ms.ToArray();

                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Resevation", Server.MapPath(@"\")));
                    string pathString = Path.Combine(originalDirectory.ToString(), "\\");

                    if (!Directory.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);

                    }

                    bitMap.Save(Server.MapPath("~/images/Resevation/" + User.Identity.Name + resId + "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
                }


                        
                System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/Resevation/") + res.Resevationcode + ".pdf", FileMode.Create);

                Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

                pdfDoc.Open();


                try
                {


                    //Top Heading
                    Chunk chunk = new Chunk(DateTime.UtcNow.AddHours(2).ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                    pdfDoc.Add(chunk);

                    //Horizontal Line
                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);


                    //Table
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    //0=Left, 1=Centre, 2=Right
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;
                    ////////
                    ///






                    //Cell no 1
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    Image image = Image.GetInstance(Server.MapPath("~/images/Resevation/" + User.Identity.Name + resId + "qrcode.png"));
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    table.AddCell(cell);




                    chunk = new Chunk("RESEVATION NUMBER: " + resId + "\nDATE: \n" + res.Date + "\nOCCASSION DATE: " + res.Occationdate +"\nSERVICE TYPE: " + res.Resevationtype + "\nSERVICE FEE: R " + res.Servicefee + "\nBALANCE DUE :R 0\nTOTAL: R " + res.Total, FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;
                    var para3 = new Paragraph(chunk);
                    para3.Alignment = Element.ALIGN_LEFT;
                    para3.Alignment = -100;

                    cell.AddElement(para3);
                    table.AddCell(cell);




                    chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;

                    var para4 = new Paragraph(chunk);
                    para4.Alignment = Element.ALIGN_LEFT;
                    para4.Alignment = -100;

                    cell.AddElement(para4);
                    table.AddCell(cell);



                    chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;

                    var para5 = new Paragraph(chunk);
                    para5.Alignment = Element.ALIGN_LEFT;
                    para5.Alignment = -100;
                    cell.AddElement(para5);
                    table.AddCell(cell);


                    chunk = new Chunk("FOOD LOVERS" + "\n" + "Restugrp07@gmail.com" + "\n" + "Durban North, South Africa" + "\n" + "durban" + "\n" + "Kwazulunatal" + "\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));

                    //Cell no 2
                    cell = new PdfPCell();
                    cell.Border = 0;

                    var para1 = new Paragraph(chunk);
                    para1.Alignment = Element.ALIGN_RIGHT;


                    cell.AddElement(para1);
                    table.AddCell(cell);



                    //Add table to document
                    pdfDoc.Add(table);

                    //Horizontal Line
                    //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    //pdfDoc.Add(line);

                    //Table
                    table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = -0f;

                    //Cell
                    cell = new PdfPCell();
                    chunk = new Chunk("RESEVATION ITEMS", FontFactory.GetFont("Daytona Condensed Light", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell.Colspan = 5;
                    var para13 = new Paragraph(chunk);
                    para13.Alignment = Element.ALIGN_CENTER;


                    cell.AddElement(para13);
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    table.AddCell("PRODUCT NUMBER");
                    table.AddCell("TITLE" + Environment.NewLine);
                    table.AddCell("PRICE" + Environment.NewLine);
                    table.AddCell("QUANTITY" + Environment.NewLine);
                    table.AddCell("TOTAL" + Environment.NewLine);
                    pdfDoc.Add(table);



                    var cart = db.Resevationmeals.Where(x => x.ResevationId == resId);

                    table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 30f;




                    foreach (var item in cart)
                    {
                        line = new Paragraph(new Chunk(item.MealId.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);
                        line = new Paragraph(new Chunk(item.Product.Name.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);

                        line = new Paragraph(new Chunk("R:" + item.Product.Price.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);
                        line = new Paragraph(new Chunk(item.Quantityordered.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);

                        line = new Paragraph(new Chunk("R:" + (item.Quantityordered * item.Product.Price).ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);


                    }


                    pdfDoc.Add(table);



                    Paragraph para = new Paragraph();
                    para.Add("THANK YOU");
                    pdfDoc.Add(para);

                    //Horizontal Line
                    line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);

                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    pdfDoc.CloseDocument();
                    fs.Close();

                    return Redirect("/Account/myprofile");
                }
                catch (Exception e)
                {
                    throw e;
                }

            }

        }


        public ActionResult Myresevations()
        {
            var res = db.Resevations.Where(x => x.Useremail == User.Identity.Name).ToList();
            return View(res);
        }



        public ActionResult Startcheckin()
        {
            return View();
        }

        public ActionResult verifyticket(string qr)
        {

            Resevation r = db.Resevations.FirstOrDefault(x => x.Resevationcode == qr && x.isResevationApproved == true && x.isCheckedin == false);

            if(r!= null)
            {
                r.isCheckedin = true;
                r.Checkindate = DateTime.Now;
                db.SaveChanges();

                return Redirect("/home/Checkedinsuccessfuly?resId="+r.Id);
            }
            else
            {
                return RedirectToAction("Invalidticket");
            }

        }



        public ActionResult Invalidticket()
        {
            return View();
        }

        public ActionResult Checkedinsuccessfuly(int?resId)
        {
            Resevation r = db.Resevations.Find(resId);

            return View(r);
        }

        [HttpGet]
        public ActionResult Cancel(Cancelation model, Nullable<int> bkid, Nullable<int> resid)
        {

            model.RestubookingId = bkid;
            model.ResevationId = resid;

            return View(model);
        }


        [HttpPost]
        public ActionResult Cancel(Cancelation model)
        {
            Cancelation c = new Cancelation()
            {
                Date = DateTime.Now,
                Cancelationreason = model.Cancelationreason,
                ResevationId = model.ResevationId,
                RestubookingId = model.RestubookingId,
                Status = "",
                Statusnum = 1,
            };
            db.Cancelations.Add(c);
            db.SaveChanges();

            if(model.ResevationId != null)
            {
                Resevation res = db.Resevations.Find(model.ResevationId);
                res.Statusnum = 14;
                res.Status = "Resevation cancelled";
                res.isCancelled = true;
                db.SaveChanges();

                if(DateTime.Now <= res.Occationdate.AddDays(3))
                {
                    Cancelation cc = db.Cancelations.Find(c.Id);
                    cc.isEligibleforrefund = true;
                    cc.Refundedamount = res.Total - (res.Total * 0.01) + res.Servicefee;
                    db.SaveChanges();
                    return Redirect("/home/Uploadproofofaccount?cid="+ c.Id);
                }

            }

            if(model.RestubookingId != null)
            {
                Resturantbooking resb = db.Resturantbookings.Find(model.RestubookingId);
                resb.Statusnum = 14;
                resb.Status = "Resevation cancelled";
                resb.isCancelled = true;
                db.SaveChanges();

                if (DateTime.Now <= resb.Occasiondate.AddDays(3))
                {
                    Cancelation cc = db.Cancelations.Find(c.Id);
                    cc.isEligibleforrefund = true;
                    cc.Refundedamount = resb.Total - (resb.Total * 0.01)+resb.Servicefee;
                    db.SaveChanges();
                    return Redirect("/home/Uploadproofofaccount?cid=" + c.Id);
                }

            }

            return Redirect("/home/Uploadproofofaccount?cid=" + c.Id);


        }

        [HttpGet]
        public ActionResult Uploadproofofaccount(int?cid)
        {
            Cancelation c = db.Cancelations.Find(cid);

            return View(c);
        }

        [HttpPost]
        public ActionResult Uploadproofofaccount(Cancelation model,HttpPostedFileBase file)
        {
            Cancelation c = db.Cancelations.Find(model.Id);
            c.Status = "Proof of account received";
            c.Statusnum = 2;
            c.Proofofaccount = file.FileName;
            db.SaveChanges();

            int id = model.Id;

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Proofs");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Proofs\\" + id.ToString());
          
            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
           

            //check if a file was  uploaded
            if (file != null && file.ContentLength > 0)
            {

                string imageName = file.FileName;

                var path = string.Format("{0}\\{1}", pathString2, imageName);

                //save original image
                file.SaveAs(path);
               

            }


                return RedirectToAction("Cancelationreceived");
        }


        public ActionResult Cancelationreceived()
        {
            return View();
        }




        [Authorize,HttpGet]
        public ActionResult Bookresturant()
        {
            return View();
        }


        [Authorize, HttpPost]
        public ActionResult Bookresturant(Resturantbooking model)
        {
            Resturantbooking rb = new Resturantbooking()
            {
                Date = DateTime.Now,
                isCancelled = false,
                Occasiondate = model.Occasiondate,
                Servicefee = 5000,
                Status = model.Status,
                Statusnum = 1,
                Total = 0,
                Playlist = model.Playlist

            };
            db.Resturantbookings.Add(rb);
            db.SaveChanges();


            return Redirect("/home/Addbookingmeals?bkid=" + rb.Id);
        }




        [HttpGet]
        public ActionResult Addbookingmeals(int?bkid)
        {
            ViewBag.bkid = bkid;
            var m = db.Products.ToList();
            return View(m);
        }









        public ActionResult Addtobookingmeal(int mealId, int bkid)
        {

            Resturantbooking rb = db.Resturantbookings.Find(bkid);

            var bkm = db.Bookingmeals.FirstOrDefault(x => x.BookingId == bkid && x.MealId == mealId);

            if (bkm != null)
            {
                Bookingmeal remm = bkm;
                remm.Quantityordered = remm.Quantityordered + 1;
                db.SaveChanges();
            }

            else
            {
                Bookingmeal bokm = new Bookingmeal()
                {
                    BookingId = bkid,
                    MealId = mealId,
                    Quantityordered = rb.Numberofguests,
                };

                db.Bookingmeals.Add(bokm);
                db.SaveChanges();
            }

            return Redirect("/home/bookingmeals?bkid=" + bkid);
        }


        public ActionResult Bookingmeals(int bkid)
        {
            ViewBag.bkid = bkid;

            var bokm = db.Bookingmeals.Where(x => x.BookingId == bkid).ToList();

            return View(bokm);
        }

        public ActionResult bkincre(int bokmealId)
        {

            Bookingmeal bokm = db.Bookingmeals.Find(bokmealId);
            bokm.Quantityordered = bokm.Quantityordered + 1;
            db.SaveChanges();

            return Redirect("/home/bookingnmeals?bkid=" + bokm.BookingId);

        }

        public ActionResult bkrem(int bokmealId)
        {

            Bookingmeal resm = db.Bookingmeals.Find(bokmealId);
            db.Bookingmeals.Remove(resm);
            db.SaveChanges();

            return Redirect("/home/bookingmeals?bkid=" + resm.BookingId);

        }

        public ActionResult bkdecre(int resmealId)
        {

            Bookingmeal resm = db.Bookingmeals.Find(resmealId);
            if (resm.Quantityordered < 2)
            {
                db.Bookingmeals.Remove(resm);
                db.SaveChanges();
            }
            else
            {
                resm.Quantityordered = resm.Quantityordered - 1;
                db.SaveChanges();
            }

            return Redirect("/home/bookingmeals?bkid=" + resm.BookingId);
        }


        public ActionResult BookingOrderdetailsandpayment(int bkid)
        {


            var resm = db.Bookingmeals.Where(x => x.BookingId == bkid).ToList();

            Resturantbooking res = db.Resturantbookings.Find(bkid);
           


            if (res.Statusnum == 1)
            {
                res.Total = resm.Sum(x => x.Product.Price * x.Quantityordered) + 5000;
                res.Servicefee = 5000;
                res.Statusnum = 2;
                res.Status = "WAITING FOR PAYMENT";
                db.SaveChanges();
            }


            return View(res);
        }


        public ActionResult paymentbookingmeals(int bkid)
        {
            var resm = db.Bookingmeals.Where(x => x.BookingId == bkid).ToList();

            return View(resm);
        }



        public ActionResult Bookingpaid(int bkid)
        {



            using (Db db = new Db())
            {


                GetQuery code = new GetQuery();

                string c = code.Main();

                Resturantbooking res = db.Resturantbookings.Find(bkid);
                res.Bookingcode = c;
                res.Statusnum = 3;
                res.Status = "PAIMENT RECEIVED";
                res.Bookingslip = c + ".pdf";
                db.SaveChanges();

                var originalDirectory1 = new DirectoryInfo(string.Format("{0}Images\\Booking", Server.MapPath(@"\")));
                var pathString11 = Path.Combine(originalDirectory1.ToString());

                if (!Directory.Exists(pathString11))
                {
                    Directory.CreateDirectory(pathString11);
                }

                string Message = res.Bookingcode;


                QRCodeGenerator ObjQr = new QRCodeGenerator();

                QRCodeData qrCodeData = ObjQr.CreateQrCode(Message, QRCodeGenerator.ECCLevel.Q);

                Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                using (MemoryStream ms = new MemoryStream())

                {

                    bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    byte[] byteImage = ms.ToArray();

                    var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Booking", Server.MapPath(@"\")));
                    string pathString = Path.Combine(originalDirectory.ToString(), "\\");

                    if (!Directory.Exists(pathString))
                    {
                        Directory.CreateDirectory(pathString);

                    }

                    bitMap.Save(Server.MapPath("~/images/Booking/" + User.Identity.Name + bkid + "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
                }



                System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/Booking/") + res.Bookingcode + ".pdf", FileMode.Create);

                Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

                pdfDoc.Open();


                try
                {


                    //Top Heading
                    Chunk chunk = new Chunk(DateTime.UtcNow.AddHours(2).ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                    pdfDoc.Add(chunk);

                    //Horizontal Line
                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);


                    //Table
                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    //0=Left, 1=Centre, 2=Right
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;
                    ////////
                    ///






                    //Cell no 1
                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    Image image = Image.GetInstance(Server.MapPath("~/images/Booking/" + User.Identity.Name + bkid + "qrcode.png"));
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    table.AddCell(cell);




                    chunk = new Chunk("BOOKING NUMBER: " + bkid + "\nDATE: \n" + res.Date + "\nOCCASSION DATE: " + res.Occasiondate + "\nSERVICE FEE: R " + res.Servicefee + "\nBALANCE DUE :R 0\nTOTAL: R " + res.Total, FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;
                    var para3 = new Paragraph(chunk);
                    para3.Alignment = Element.ALIGN_LEFT;
                    para3.Alignment = -100;

                    cell.AddElement(para3);
                    table.AddCell(cell);




                    chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;

                    var para4 = new Paragraph(chunk);
                    para4.Alignment = Element.ALIGN_LEFT;
                    para4.Alignment = -100;

                    cell.AddElement(para4);
                    table.AddCell(cell);



                    chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;

                    var para5 = new Paragraph(chunk);
                    para5.Alignment = Element.ALIGN_LEFT;
                    para5.Alignment = -100;
                    cell.AddElement(para5);
                    table.AddCell(cell);


                    chunk = new Chunk("FOOD LOVERS" + "\n" + "Restugrp07@gmail.com" + "\n" + "Durban North, South Africa" + "\n" + "durban" + "\n" + "Kwazulunatal" + "\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));

                    //Cell no 2
                    cell = new PdfPCell();
                    cell.Border = 0;

                    var para1 = new Paragraph(chunk);
                    para1.Alignment = Element.ALIGN_RIGHT;


                    cell.AddElement(para1);
                    table.AddCell(cell);



                    //Add table to document
                    pdfDoc.Add(table);

                    //Horizontal Line
                    //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    //pdfDoc.Add(line);

                    //Table
                    table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = -0f;

                    //Cell
                    cell = new PdfPCell();
                    chunk = new Chunk("RESEVATION ITEMS", FontFactory.GetFont("Daytona Condensed Light", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                    cell.Colspan = 5;
                    var para13 = new Paragraph(chunk);
                    para13.Alignment = Element.ALIGN_CENTER;


                    cell.AddElement(para13);
                    cell.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell);

                    table.AddCell("PRODUCT NUMBER");
                    table.AddCell("TITLE" + Environment.NewLine);
                    table.AddCell("PRICE" + Environment.NewLine);
                    table.AddCell("QUANTITY" + Environment.NewLine);
                    table.AddCell("TOTAL" + Environment.NewLine);
                    pdfDoc.Add(table);



                    var cart = db.Bookingmeals.Where(x => x.BookingId == bkid);

                    table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 30f;




                    foreach (var item in cart)
                    {
                        line = new Paragraph(new Chunk(item.MealId.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);
                        line = new Paragraph(new Chunk(item.Product.Name.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);

                        line = new Paragraph(new Chunk("R:" + item.Product.Price.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);
                        line = new Paragraph(new Chunk(item.Quantityordered.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);

                        line = new Paragraph(new Chunk("R:" + (item.Quantityordered * item.Product.Price).ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                        table.AddCell(line);


                    }


                    pdfDoc.Add(table);



                    Paragraph para = new Paragraph();
                    para.Add("THANK YOU");
                    pdfDoc.Add(para);

                    //Horizontal Line
                    line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);

                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    pdfDoc.CloseDocument();
                    fs.Close();

                    return Redirect("/Account/myprofile");
                }
                catch (Exception e)
                {
                    throw e;
                }

            }

        }


        public ActionResult Mybookings()
        {
            var res = db.Resevations.Where(x => x.Useremail == User.Identity.Name).ToList();
            return View(res);
        }






    }

}