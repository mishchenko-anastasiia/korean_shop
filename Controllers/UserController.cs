using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Shop.Controllers
{
    public class UserController : BaseController
    {
        [HttpGet]
        public ActionResult SignIn()
        {
            if (IsAuthorized()) 
                return Redirect("/Home/Index");

            return View();
        }

        [HttpGet]
        public ActionResult Orders()
        {
            User user = GetUser();

            if (DetectClientRole() != Role.USER || user == null)
                return Redirect("~/Home/Index");

            ViewBag.Orders = db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails
                .Select(od => od.Product))
                .Where(o => o.UserId == user.Id);

            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            if (IsAuthorized())
                return Redirect("/Home/Index");

            return View();
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            base.LogOut();
            Session["cart"] = null;
            return Redirect("/Home/Index");
        }

        [HttpPost]
        public ActionResult SignIn(User user)
        {
            if (ModelState.IsValid)
            {
                if (SignIn(user.Login, user.Password))
                {
                    return Redirect("/Home/Index");
                }
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                if (SignUp(user.Login, user.Password))
                {
                    return Redirect("/Home/Index");
                }
            }

            return View(user);
        }
    }
}