using Budget_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyBudget.Controllers
{
    public class LoginController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            User data = db.User.Where(x => x.Email == model.Email && x.Password == model.Password).SingleOrDefault();
            if (data != null)
            {
                CurrentSession.Set<User>("login", data);
                return RedirectToAction("/", "Home");

            }
            return View(model);


        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            model.IsDelete = false;
            model.IsActive = true;
            model.CreatedDate = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            model.CreatedUser = "system";
            db.User.Add(model);
            db.SaveChanges();
            return RedirectToAction("Login", "Login");
        }
        public ActionResult LogOut()
        {
            if (CurrentSession.User != null)
            {
                CurrentSession.Clear();
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }
    }
}