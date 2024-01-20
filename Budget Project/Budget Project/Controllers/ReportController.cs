using Budget_Project.Filters;
using Budget_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Budget_Project.Controllers
{
    [Auth]
    public class ReportController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();

        // GET: Report
        public ActionResult Index()
        {

            return View();
        }

        public class CategoryChartVM
        {
            public List<string> lables { get; set; }
            public List<int> names { get; set; }
        }

        [HttpGet]
        public JsonResult AjaxMethod()
        {
            var currentUser = CurrentSession.User.Id;
            var category = db.Category.Where(data => data.UserId == currentUser).ToList();

            List<string> lables = new List<string>();
            List<int> names = new List<int>();
            foreach (var item in category)
            {
                var count = db.Transaction.Count(x => x.UserId == currentUser && x.CategoryId == item.Id);
                if (count > 0)
                {
                    names.Add(count);
                }
            }

            for (int i = 0; i < category.Count(); i++)
            {
                var asd = category[i].Id;
                var trans = db.Transaction.Where(data => data.UserId == currentUser && data.CategoryId == asd).ToList();

                if (trans.Count() > 0)
                {
                    lables.Add(category[i].Name);
                }
            }

            CategoryChartVM categoryChartVM = new CategoryChartVM
            {
                names = names,
                lables = lables
            };


            return Json(categoryChartVM, JsonRequestBehavior.AllowGet);
        }
    }
}