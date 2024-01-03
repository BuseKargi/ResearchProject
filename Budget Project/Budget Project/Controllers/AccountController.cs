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
    }
}