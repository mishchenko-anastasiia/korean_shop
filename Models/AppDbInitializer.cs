using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Shop.Models
{
    public class AppDbInitializer : DropCreateDatabaseAlways<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            User admin = new User { Login = "Admin", Password = "password", Role = Role.ADMIN };
            User user = new User { Login = "Nastya", Password = "neko69", Role = Role.USER };

            Brand theHistoryOfWhoo = new Brand { Name = "THE HISTORY OF WHOO" };
            Category giftSet = new Category { Name = "GIFT SET" };

            Product p1 = new Product
            {
                Name = "THE HISTORY OF WHOO GONGJINHYANG SPECIAL GIFT SET",
                Brand = theHistoryOfWhoo,
                Category = giftSet,
                Price = 395,
                ImagePath = "THW014S-600x600.jpg"
            };

            context.Products.Add(p1);

            context.Users.Add(admin);
            context.Users.Add(user);

            context.SaveChanges();
        }
    }
}