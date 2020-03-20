using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Shop.Models;

namespace Shop.Controllers
{
    public class ManageController : BaseController
    {
        public ActionResult Index()
        {
            if (DetectClientRole() != Role.ADMIN)
                return Redirect("~/Home/Index");

            ViewBag.Products = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand);

            return View();
        }

        public ActionResult Orders()
        {
            ViewBag.Orders = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails
                .Select(od => od.Product));

            return View();
        }

        public ActionResult Edit(int ProductId)
        {
            if (DetectClientRole() != Role.ADMIN)
                return Redirect("~/Home/Index");

            ViewBag.Brands = db.Brands;
            ViewBag.Categories = db.Categories;

            Product product = db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Id == ProductId).
                FirstOrDefault();

            if (product != null)
            {
                return View(product);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int ProductId)
        {
            if (DetectClientRole() != Role.ADMIN)
                return Redirect("~/Home/Index");

            ViewBag.Brands = db.Brands;
            ViewBag.Categories = db.Categories;

            Product product = db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Id == ProductId).
                FirstOrDefault();

            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            if (DetectClientRole() != Role.ADMIN)
                return Redirect("~/Home/Index");

            ViewBag.Brands = db.Brands;
            ViewBag.Categories = db.Categories;

            return View();
        }

        [HttpPost]
        public ActionResult Save(string Name, int? Price, HttpPostedFileBase Image, string Category, string Brand)
        {
            if (DetectClientRole() != Role.ADMIN)
                return Redirect("~/Home/Index");

            try
            {
                int ProductId = int.Parse(Request.UrlReferrer.Query.Split('=').LastOrDefault());

                Product product = db.Products.Where(p => p.Id == ProductId).FirstOrDefault();
                Category category = db.Categories.Where(c => c.Name == Category).FirstOrDefault();
                Brand brand = db.Brands.Where(b => b.Name == Brand).FirstOrDefault();

                if (product == null || Name == "" || Price == null || Price <= 0 || category == null || brand == null)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }

                product.Name = Name;
                product.Price = (int)Price;
                product.Category = category;
                product.Brand = brand;

                if (Image != null)
                {
                    string imageName = System.IO.Path.GetFileName(Image.FileName);
                    string imagePath = System.IO.Path.Combine(Server.MapPath("~/Images/Products"), imageName);

                    Image.SaveAs(imagePath);

                    product.ImagePath = imageName;
                }

                db.SaveChanges();
            }
            catch (Exception e) 
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return Redirect("~/Manage");
        }

        [HttpPost]
        public ActionResult Create(string Name, int? Price, HttpPostedFileBase Image, string Category, string Brand)
        {
            if (DetectClientRole() != Role.ADMIN)
                return Redirect("~/Home/Index");

            try
            {
                Category category = db.Categories.Where(c => c.Name == Category).FirstOrDefault();
                Brand brand = db.Brands.Where(b => b.Name == Brand).FirstOrDefault();

                if (Name == "" || Price == null || Price <= 0 || Image == null || category == null || brand == null)
                {
                    return Redirect(Request.UrlReferrer.ToString());
                }

                string imageName = System.IO.Path.GetFileName(Image.FileName);
                string imagePath = System.IO.Path.Combine(Server.MapPath("~/Images/Products"), imageName);

                Image.SaveAs(imagePath);

                Product product = new Product
                {
                    Name = Name,
                    Price = (int)Price,
                    ImagePath = imageName,
                    Category = category,
                    Brand = brand
                };

                db.Products.Add(product);

                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return Redirect("~/Manage");
        }
    }
}