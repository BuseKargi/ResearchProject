using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using Budget_Project.Filters;
using System.Web.Mvc;
using Budget_Project.Models;

namespace MyBudget.Controllers
{
    public class CategoryController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();
        // GET: Category
        [Auth]
        public ActionResult Index()
        {
            var currentUser = CurrentSession.User.Id;
            var category = db.Category.Where(x => x.UserId == currentUser).ToList();
            return View(category);
        }
    }
}