using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace Shop.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext() : base("Shop")
        {
        }

        static AppDbContext()
        {
            bool resetDatabase = false;
            bool executeOnce = false;

            if (resetDatabase)
            {
                Database.SetInitializer(new AppDbInitializer());
            }

            if (executeOnce)
            {
                var context = new AppDbContext();
                ExecuteOnce(context);
                context.SaveChanges();
                MessageBox.Show("Do not forget to set 'executeOnce = false'");
                Environment.Exit(0);
            }
        }

        private static void ExecuteOnce(AppDbContext context)
        {
            context.Categories.Add(new Category { Name = "Test Category" });
        }

        public bool UserExists(String login, String password)
        {
            return Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault() != null;
        }

        public bool CreateUser(String login, String password, Role role = Role.USER)
        {
            if (UserExists(login, password))
                return false;

            Users.Add(new User { Login = login, Password = password, Role = role });
            SaveChanges();

            return UserExists(login, password);
        }

        public User FindUser(String login, String password)
        {
            return Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();
        }
    }
}