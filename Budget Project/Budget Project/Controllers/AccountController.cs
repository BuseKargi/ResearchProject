using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Budget_Project.Filters;
using Budget_Project.Models;

namespace Budget_Project.Controllers
{
    public class AccountController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();

        // GET: Account
        [Auth]
        public ActionResult Index()
        {
            var currentUser = CurrentSession.User.Id;
            var account = db.Account.Where(x => x.UserId == currentUser).ToList();
            return View(account);
        }
        [Auth]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Account model, HttpPostedFileBase LogoPath)
        {
            if (LogoPath != null)
            {
                string _LogoPath = $"{model.Name}.{LogoPath.ContentType.Split('/')[1]}";
                LogoPath.SaveAs(Server.MapPath($"~/Assets/images/account/{_LogoPath}"));
                model.Logo = _LogoPath;
            }
            else
            {
                model.Logo = null;
            }
            var currentUser = CurrentSession.User.Id;

            model.CreatedDate = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            model.UserId = currentUser;
            db.Account.Add(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id != null)
            {
                Account data = db.Account.SingleOrDefault(x => x.Id == id);
                if (data != null)
                {
                    return View(data);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Account model, HttpPostedFileBase LogoPath)
        {
            Account data = db.Account.SingleOrDefault(x => x.Id == model.Id);
            if (data != null)
            {
                if (LogoPath != null)
                {
                    string _LogoPath = $"{model.Name}.{LogoPath.ContentType.Split('/')[1]}";
                    LogoPath.SaveAs(Server.MapPath($"~/Assets/images/account/{_LogoPath}"));
                    data.Logo = _LogoPath;
                }
                else
                {
                    model.Logo = null;
                }
                data.Name = model.Name;
                data.ModifiedDate = DateTime.Now;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (id != null)
            {
                Account data = db.Account.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    db.Account.Remove(data);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
    }
}