using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Restugrp07.Models
{
    public class Models
    {
    }
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserRole
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        public int RoleId { get; set; }


        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Display(Name = "First Name(s)")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]

        public string LastName { get; set; }
        [Display(Name ="Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name ="Phone")]
        public string PhoneNumber { get; set; }

        public int Credit { get; set; }

        public string Status { get; set; }
        public string AvailabilityStatus { get; set; }



    }



    public class GetQuery
    {

        public string Main()
        {
            int length = 20;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }
    }

    public class Verify
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }
        public string Code { get; set; }

    }


    public class Address
    {
        [Key]

        public int Id { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int Zip { get; set; }
        public string Addres { get; set; }
        public string Username { get; set; }
    }



    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameFix { get; set; }

    }
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }

        public string Query { get; set; }
        [Required]
        public int Quantity { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        [Required]

        public int Price { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }
        public int CategoryId { get; set; }


        public int Statusnum { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }



    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int DeliveryFee { get; set; }
        [Display(Name = "Date")]

        public DateTime CreatedAt { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        [Display(Name = "Order code")]

        public string OrderCode { get; set; }

        [Display(Name = "Total price")]
        public int Statusnum { get; set; }
        public int TotalPrice { get; set; }

        public int Ordertypenum { get; set; }

        public string Ordertype { get; set; }
        public int Due { get; set; }

        public Nullable<DateTime> Expierdate { get; set; }

        public int tems { get; set; }

        public int couter { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }


    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Product2Id { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public int Producttypenum { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Orders { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }


    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Qr { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SUrbace { get; set; }
        public int ZipCode { get; set; }
        public string Servicetype { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime ServicingDate { get; set; }
        public string Satatus { get; set; }
        public string Constractor { get; set; }
        public string InspectinStatus { get; set; }
        public string ServiceStatus { get; set; }
        public int Total { get; set; }

        public int StatusNum { get; set; }
    }

    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Registration")]
        public string NumberPlate { get; set; }
        [Display(Name = "Driver email")]
        public string DriverEmail { get; set; }
        public string Status { get; set; }
    }

    public class Delivery
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int VehicleId { get; set; }
        [Display(Name = "Vehicle")]
        public string NumberPlate { get; set; }
        public int DriverId { get; set; }
        [Display(Name = "Driver Username")]
        public string DriverName { get; set; }
        public string Destination { get; set; }
        public DateTime PickUpTime { get; set; }
        public string PickUpAddress { get; set; }
        public DateTime Date { get; set; }

        public string DriverConfirm { get; set; }

        [ForeignKey("DriverId")]
        public virtual User Driver { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }
    }


    public class PickPoint
    {
        [Key]
        public int Id { get; set; }
        public string PointAddress { get; set; }
        public string DriverEmail { get; set; }
        public string PickUpPhone { get; set; }
        public int PickVehId { get; set; }
        public string NumberPlate { get; set; }
    }

    public class Refund
    {
        [Key]
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Destination { get; set; }
        public int statusnum { get; set; }
        public string PickupAddress { get; set; }
        public int OrderNum { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

    }



    public class Partrequest
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "User email")]
        public string Useremail { get; set; }

        [Display(Name = "Part name")]
        public string Partnumber { get; set; }
        [Display(Name = "Part image")]

        public string Partimage { get; set; }
        [Display(Name = "Vehicle name")]

        public string Partcarmodel { get; set; }
        [AllowHtml]
        [Display(Name = "Request Feedback message")]
        public string Feedback { get; set; }
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public string Status { get; set; }
        public int Statusnum { get; set; }

        public int Orderid { get; set; }

        public string Condition { get; set; }
    }


    public class Tradein
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Item name")]
        public string Itemname { get; set; }
        [Display(Name = "Item description")]
        public string Itemdescription { get; set; }
        [Display(Name = "Value in Rands(R)")]
        public int Total { get; set; }

        public string Status { get; set; }

        public int Statusnum { get; set; }
        [Display(Name = "Item image")]
        public string Itemimage { get; set; }
        [Display(Name = "User email")]
        public string Useremail { get; set; }

        [Display(Name = "Assessor email")]
        public string Assessoremail { get; set; }

        public int AssessorId { get; set; }
        public string assessoersignature { get; set; }

        [Display(Name = "Proof of ownership")]
        public string Proofofownership { get; set; }
        [Display(Name = "Id copy")]
        public string Idcopy { get; set; }
        [Display(Name = "Travel fee")]
        public int Travelfee { get; set; }
        [Display(Name = "Purchase date")]
        public DateTime Purchasedate { get; set; }

        [Display(Name = "Request assessor")]
        public bool Requestassessor { get; set; }
        [Display(Name = "Full address")]
        public string Address { get; set; }

        public string customersignature { get; set; }

    }

    public class Resevation
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Display(Name ="Occassion date")]
        public DateTime Occationdate { get; set; }

        [Display(Name = "Number of guests")]
        [Range(minimum: 1, maximum: 10)]

        public int Numberofguests { get; set; }

        [Display(Name = "Reservation type")]

        public string Resevationtype { get; set; }

        public string Status { get; set; }

        public int Statusnum { get; set; }

        [Display(Name = "Table layout")]

        public string Tablelayout { get; set; }

        [Display(Name = "Theme color")]

        public string Themecolor { get; set; }
        [Display(Name = "User email")]

        public string Useremail { get; set; }
        [Display(Name = "Resevation code")]
        public string Resevationcode { get; set; }
        [Display(Name = "Resevation slip")]

        public string Resavationslip { get; set; }

        public int Total { get; set; }
        [Display(Name = "Service fee")]

        public int Servicefee { get; set; }
        [Display(Name = "Is resevation approved")]

        public bool isResevationApproved { get; set; }
        [Display(Name = "Is Processd?")]

        public bool isProcessed { get; set; }
        [Display(Name = "Admins signature")]

        public string Adminsignature { get; set; }
        [Display(Name ="Waiter email")]
        public string Waiteremail { get; set; }
        [Display(Name ="is User checkedin?")]
        public bool isCheckedin { get; set; }
        [Display(Name ="Checkin date")]
        public Nullable<DateTime> Checkindate { get; set; }
        [Display(Name = "is Resevation cancelled")]
        public bool isCancelled { get; set; }

        public IEnumerable<SelectListItem> Waiters { get; set; }
    }



    public class Resevationmeal
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Meal / Item number")]

        public int MealId { get; set; }
        [Display(Name ="Quantity ordered")]

        public int Quantityordered { get; set; }
        [Display(Name = "Resevation number")]

        public int ResevationId { get; set; }

        public bool isAvailable { get; set; }

        public bool isChecked { get; set; }

        [ForeignKey("ResevationId")]
        public virtual Resevation Resevation { get; set; }

        [ForeignKey("MealId")]
        public virtual Product Product { get; set; }
    }


    public class Cancelation
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        [Display(Name ="Cancelation reason")]
        public string Cancelationreason { get; set; }
        [Display(Name = "Resevation number ")]

        public Nullable<int> ResevationId { get; set; }
        [Display(Name = "Booking number")]

        public Nullable<int> RestubookingId { get; set; }

        public string Status { get; set; }

        public int Statusnum { get; set; }
        [Display(Name = "Refunded amount")]

        public double Refundedamount { get; set; }
        [Display(Name = "is Eligible for refund")]

        public bool isEligibleforrefund { get; set; }
        [Display(Name = "Proff of bank account")]

        public string Proofofaccount { get; set; }

        [ForeignKey("ResevationId")]
        public virtual Resevation Resevation { get; set; }

        [ForeignKey("RestubookingId")]
        public virtual Resturantbooking Resturantbooking { get; set; }
    }


    public class Resturantbooking
    {
        [Key]
        public int Id { get; set; }

        public string Status { get; set; }

        public int Statusnum { get; set; }

        public DateTime Date { get; set; }
        [Display(Name = "Is cancelled?")]

        public bool isCancelled { get; set; }
        [Display(Name = "Occasion date")]

        public DateTime Occasiondate { get; set; }

        public int Total { get; set; }

        [Display(Name = "Service fee")]

        public int Servicefee { get; set; }

        [Display(Name = "Theme color")]

        public string Themecolor { get; set; }
        [Display(Name = "User email")]

        public string Useremail { get; set; }
        [Display(Name = "Resevation code")]
        public string Bookingcode { get; set; }
        [Display(Name = "Resevation slip")]

        public string Bookingslip { get; set; }
        [Display(Name = "Number of guests")]

        public int Numberofguests { get; set; }

        public bool isBookingApproved { get; set; }
        [Display(Name = "Is Processd?")]

        public bool isProcessed { get; set; }
        [Display(Name = "Admins signature")]

        public string Adminsignature { get; set; }
        [Display(Name = "Waiter email")]
        public string Waiteremail { get; set; }
        [Display(Name = "is User checkedin?")]
        public bool isCheckedin { get; set; }
        [Display(Name = "Checkin date")]
        public Nullable<DateTime> Checkindate { get; set; }

        public string Playlist { get; set; }


    }





    public class Bookingmeal
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Meal / Item number")]

        public int MealId { get; set; }
        [Display(Name = "Quantity ordered")]

        public int Quantityordered { get; set; }
        [Display(Name = "Booking number")]

        public int BookingId { get; set; }

        public bool isAvailable { get; set; }

        public bool isChecked { get; set; }

        [ForeignKey("BookingId")]
        public virtual Resturantbooking Resturantbooking { get; set; }

        [ForeignKey("MealId")]
        public virtual Product Product { get; set; }
    }







    public class Resturantcalender
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime Occasiondate { get; set; }

        public int Bookingid { get; set; }

        [ForeignKey("Bookingid")]

        public virtual Booking Booking { get; set; }

    }



}