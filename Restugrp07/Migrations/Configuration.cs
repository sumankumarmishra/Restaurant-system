namespace Restugrp07.Migrations
{
    using Restugrp07.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Restugrp07.Models.Db>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Restugrp07.Models.Db context)
        {
            using (Db db = new Db())
            {
                if (db.Roles.Any(x => x.Name.Equals("Admin")))
                {

                }
                else
                {

                    Role Role1 = new Role()

                    {

                        Name = "Admin",


                    };

                    Role Role2 = new Role()

                    {
                        Name = ("User"),

                    };



                    Role Role3 = new Role()

                    {
                        Name = ("Driver"),

                    };
                    Role Role4 = new Role()

                    {
                        Name = ("Assessor"),

                    };
                    Role Role5 = new Role()

                    {
                        Name = ("Waiter"),

                    };


                    db.Roles.Add(Role1);
                    db.Roles.Add(Role2);
                    db.Roles.Add(Role3);
                    db.Roles.Add(Role4);
                    db.Roles.Add(Role5);


                    db.SaveChanges();

                    UserRole userRoles = new UserRole()

                    {
                        RoleId = 1
                    };

                    db.UserRoles.Add(userRoles);
                    db.SaveChanges();
                    int id = userRoles.UserId;

                    User userDTO = new User()
                    {
                        Id = id,
                        FirstName = ("Admin"),
                        LastName = ("Admin"),
                        EmailAddress = "Admin@gmail.com",
                        Password = ("Admin@123"),
                        PhoneNumber = "0000000000",
                        Status = "VERIFIED"

                    };
                    db.Users.Add(userDTO);
                    db.SaveChanges();

                }

            }
        }
    }
}
