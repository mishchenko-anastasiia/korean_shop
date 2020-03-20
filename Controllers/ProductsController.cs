using Shop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Shop.Controllers
{
    public class ProductsController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Role = DetectClientRole();

            ViewBag.Products = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand);

            ViewBag.Brands = db.Brands;
            ViewBag.Categories = db.Categories;

            return View();
        }

        public ActionResult Search(string Filter, string Brand, string Category)
        {
            var products = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand);

            if (Filter != "")
            {
                products = products.Where(p => p.Name.ToLower().Contains(Filter.ToLower()));
            }

            if (Brand != "All")
            {
                Brand brand = db.Brands.Where(b => b.Name == Brand).FirstOrDefault();
                products = products.Where(p => p.BrandId == brand.Id);
            }

            if (Category != "All")
            {
                Category category = db.Categories.Where(c => c.Name == Category).FirstOrDefault();
                products = products.Where(p => p.CategoryId == category.Id);
            }

            ViewBag.Brands = db.Brands;
            ViewBag.Categories = db.Categories;
            ViewBag.Products = products;

            return View("Index");
        }

        public ActionResult Cart()
        {
            if (DetectClientRole() != Role.USER)
                return Redirect("~/Home/Index");

            int total = 0;

            if (Session["cart"] != null)
            {
                foreach (var detail in (List<OrderDetail>)Session["cart"])
                {
                    total += detail.Product.Price * detail.Amount;
                }
            }

            ViewBag.Total = total;

            return View();
        }

        public ActionResult Purchase()
        {
            if (DetectClientRole() != Role.USER || Session["cart"] == null)
                return Redirect("~/Home/Index");

            return View();
        }

        [HttpPost]
        public ActionResult Confirm(string Address, string Number)
        {
            if (DetectClientRole() != Role.USER)
                return Redirect("~/Home/Index");

            if (Address == "" || Number == "")
                return Redirect(Request.UrlReferrer.ToString());

            List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];
            List<OrderDetail> OrderDetails = new List<OrderDetail>();

            int total = 0;

            foreach (var detail in cart)
            {
                total += detail.Amount * detail.Product.Price;
                Product product = db.Products.Where(p => p.Id == detail.ProductId).FirstOrDefault();
                OrderDetail orderDetail = new OrderDetail { Amount = detail.Amount, Product = product };
                db.OrderDetails.Add(orderDetail);
                OrderDetails.Add(orderDetail);
            }

            Order order = new Order { User = GetUser(), OrderDetails = OrderDetails, Total = total, Address = Address, Number = Number };

            db.Orders.Add(order);
            db.SaveChanges();

            Session["cart"] = null;

            return Redirect("~/User/Orders");
        }

        public ActionResult AddToCart(int ProductId)
        {
            Product product = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Id == ProductId).FirstOrDefault();

            if (product == null || DetectClientRole() != Role.USER)
                return Redirect("~/Home/Index");

            if (Session["cart"] == null)
            {
                List<OrderDetail> cart = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = product.Id, Product = product, Amount = 1 }
                };
                Session["cart"] = cart;
            }
            else
            {
                List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];

                OrderDetail detail = cart.Where(p => p.ProductId == ProductId).FirstOrDefault();

                if (detail != null)
                {
                    ++cart[cart.IndexOf(detail)].Amount;
                }
                else
                {
                    cart.Add(new OrderDetail { ProductId = product.Id, Product = product, Amount = 1 });
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult RemoveFromCart(int ProductId)
        {
            Product product = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Id == ProductId).FirstOrDefault();

            if (product == null || DetectClientRole() != Role.USER)
                return Redirect("~/Home/Index");

            if (Session["cart"] == null)
            {
                return Redirect("~/Home/Index");
            }
            else
            {
                List<OrderDetail> cart = (List<OrderDetail>)Session["cart"];

                OrderDetail detail = cart.Where(p => p.ProductId == ProductId).FirstOrDefault();

                if (detail != null)
                {
                    int amount = --cart[cart.IndexOf(detail)].Amount;
                    
                    if (amount == 0)
                    {
                        cart.Remove(detail);

                        if (cart.Count == 0)
                        {
                            Session["cart"] = null;
                        }
                    }
                }
            }
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}