using Restugrp07.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Restugrp07.Controllers
{
    public class DriverController : Controller
    {
        Db db = new Db();


        [Authorize]
        public ActionResult NewDeliveries()
        {

            List<DeliveryVM> de = new List<DeliveryVM>();


            var del = db.Deliveries.Where(x => x.DriverName == User.Identity.Name);


            foreach (var item in del)
            {
                Delivery model = db.Deliveries.Find(item.Id);

                Order o = db.Orders.Find(model.OrderId);

                de.Add(new DeliveryVM
                {
                    Id = model.Id,
                    OrderId = model.OrderId,
                    VehicleId = model.VehicleId,
                    NumberPlate = model.NumberPlate,
                    DriverId = model.DriverId,
                    DriverName = model.DriverName,
                    Destination = model.Destination,
                    PickUpTime = model.PickUpTime,
                    PickUpAddress = model.PickUpAddress,
                    Date = model.Date,
                    DriverConfirm = model.DriverConfirm,
                    Orderstatusnum = o.Statusnum
                });
            }

            return View(de);
        }

        public ActionResult StartDelivery(int id)
        {
            Delivery del = db.Deliveries.Find(id);

            Order ord = db.Orders.Find(del.OrderId);
            ord.Statusnum = 4;
            ord.Status = "OUT FOR DELIVERY";
            db.SaveChanges();


            string _sender = "21705746@dut4life.ac.za";
            string _password = "Dut981217";

            string recipient = ord.User.EmailAddress;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
            new System.Net.NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;
            try
            {
                var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                mail.Subject = "ORDER OUT FOR DELIVERY";
                mail.Body = "<HTML><BODY><p>Plese NOTE: Your Order is out for Delivery</p><br><br>" + "<br />DATE and TIME: " + "</ div > " + " <br></BODY></HTML>";
                mail.IsBodyHtml = true;
                //client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }



            return View(del);
        }

        [HttpGet]
        public ActionResult SignDelivery(int id)
        {
            Delivery del = db.Deliveries.Find(id);

            return View(del);
        }





        public ActionResult SignDeliveryPost(string email, int delid, int id)
        {

            var orders = db.Orders.Where(x => x.OrderId == id && x.User.EmailAddress == email && x.OrderId ==id).FirstOrDefault();
            Delivery del = db.Deliveries.Find(delid);
            if (orders != null)
            {
                int ordidd = orders.OrderId;

                Order order = db.Orders.Find(ordidd);
                order.Status = "DELIVERED";
                order.Statusnum = 5;
                db.SaveChanges();

                Vehicle vh = db.Vehicles.Find(del.VehicleId);
                vh.Status = "AVAILABLE";
                db.SaveChanges();

                User d = db.Users.Find(del.DriverId);
                d.AvailabilityStatus = "AVAILABLE";
                db.SaveChanges();




                return Redirect("/Driver/SignResult?id=" + id);
            }
            else
            {
                return Redirect("/Driver/Signfailed?id=" + id);
            }


        }



        public ActionResult SignResult(int id)
        {
            return View();
        }


        public ActionResult Signfailed(int id)
        {
            return View();
        }


        public ActionResult PickUpOrder(int id)
        {
            Delivery del = db.Deliveries.Find(id);
            return View(del);
        }



        public ActionResult ScanQr(int id)
        {
            ViewBag.Id = id;
            return View();
        }


    }
}