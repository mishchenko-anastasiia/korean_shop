using Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop.Controllers
{
    public class BaseController : Controller
    {
        private const String LOGIN_COOKIE_NAME = "login";
        private const String PASSWORD_COOKIE_NAME = "password";

        protected AppDbContext db;

        public BaseController()
        {
            ViewBag.Role = DetectClientRole();
            ViewBag.User = GetUser();
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            db = new AppDbContext();
            ViewBag.Role = DetectClientRole();
            ViewBag.User = GetUser();
        }

        protected bool SignIn(String login, String password)
        {
            if (db.UserExists(login, password))
            {
                HttpCookie loginCookie = new HttpCookie(LOGIN_COOKIE_NAME, login);
                HttpCookie passwordCookie = new HttpCookie(PASSWORD_COOKIE_NAME, password);

                Response.Cookies.Add(loginCookie);
                Response.Cookies.Add(passwordCookie);

                ViewBag.Role = DetectClientRole();

                return true;
            }

            return false;
        }

        protected bool SignUp(String login, String password)
        {
            if (db.CreateUser(login, password))
                return SignIn(login, password);

            return false;
        }

        protected void LogOut()
        {
            HttpCookie loginCookie = new HttpCookie(LOGIN_COOKIE_NAME, string.Empty);
            HttpCookie passwordCookie = new HttpCookie(PASSWORD_COOKIE_NAME, string.Empty);

            loginCookie.Expires = DateTime.Now.AddDays(-1);
            passwordCookie.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(loginCookie);
            Response.Cookies.Add(passwordCookie);
        }

        protected User GetUser()
        {
            try
            {
                String login = Request.Cookies[LOGIN_COOKIE_NAME].Value;
                String password = Request.Cookies[PASSWORD_COOKIE_NAME].Value;

                return db.FindUser(login, password);
            }
            catch (Exception e) { }

            return null;
        }

        protected Role DetectClientRole()
        {
            User user = GetUser();

            if (user != null)
                return user.Role;

            return Role.ANONYMOUS;
        }

        protected bool IsAuthorized()
        {
            return DetectClientRole() != Role.ANONYMOUS;
        }
    }
}