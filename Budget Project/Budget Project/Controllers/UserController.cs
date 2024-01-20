using Budget_Project.Filters;
using Budget_Project.Infrastructure;
using Budget_Project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Project.Controllers
{
    [Auth]
    public class UserController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();
        // GET: User
        public ActionResult Index()
        {
            var user = db.User.SingleOrDefault(x => x.Id == CurrentSession.User.Id);
            if (user!=null)
            {
                var transactions = db.Transaction.Where(x => x.UserId == user.Id).ToList();
                if (transactions.Count>0)
                {
                    ViewBag.transactions = transactions;
                }
                var currentDate = DateTime.Now;
                ViewBag.totalExpense = String.Format("{0:0.00}", db.Transaction.Where(x => x.UserId == CurrentSession.User.Id).Sum(x => x.Amount))  + " "+Defaults.MoneyType;
                ViewBag.totalThisMonthExpense = String.Format("{0:0.00}", db.Transaction.Where(x => x.UserId == CurrentSession.User.Id && x.CreatedDate.Value.Month == currentDate.Month && x.CreatedDate.Value.Year == currentDate.Year).Sum(x => x.Amount)) + " "+Defaults.MoneyType;
                ViewBag.totalThisMonthName = currentDate.ToString("MMMM");
            }
            return View(user);
        }
        public ActionResult Edit(int? id) 
        {
            if (id!=null)
            {
                var user = db.User.SingleOrDefault(x => x.Id == id);
                if (user!=null)
                {
                    return View(user);
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Edit(User model, HttpPostedFileBase LogoPath)
        {
            User data = db.User.SingleOrDefault(x => x.Id == model.Id);
            if (data!=null)
            {
                if (LogoPath!=null)
                {
                    string _LogoPath = $"{model.Fullname}.{LogoPath.ContentType.Split('/')[1]}";
                    LogoPath.SaveAs(Server.MapPath($"~/Assets/images/user/{_LogoPath}"));
                    data.ProfileImage = _LogoPath;
                }
                else
                {
                    model.ProfileImage = null;
                }
                data.Fullname = model.Fullname;
                data.Email = model.Email;
                data.Password = model.Password;
                data.ModifiedDate = model.ModifiedDate;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Delete(int id)
        {
            if (id != null)
            {
                User data = db.User.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    db.User.Remove(data);
                    db.SaveChanges();
                    return RedirectToAction("Login","Login");
                }
            }
            return RedirectToAction("Login","Login");
        }
    }
}