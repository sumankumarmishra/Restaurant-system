using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Restugrp07.Models
{
    public class Db : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Verify> Verifies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<PickPoint> PickPoints { get; set; }
        public DbSet<Refund> Refunds { get; set; }
        public DbSet<Partrequest> Partrequests { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Tradein> Tradeins { get; set; }

        public DbSet<Resevation> Resevations { get; set; }

        public DbSet<Resevationmeal> Resevationmeals { get; set; }
        public DbSet<Resturantbooking> Resturantbookings { get; set; }
        public DbSet<Bookingmeal> Bookingmeals { get; set; }

        public DbSet<Cancelation> Cancelations { get; set; } 
        public DbSet<Resturantcalender> Resturantcalenders { get; set; }


    }
}