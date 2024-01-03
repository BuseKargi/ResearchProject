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
    }
}