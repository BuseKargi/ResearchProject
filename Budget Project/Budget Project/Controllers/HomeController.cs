using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Budget_Project.Filters;
using Budget_Project.Infrastructure;
using Budget_Project.Models;
using Microsoft.Ajax.Utilities;

namespace Budget_Project.Controllers
{
    [Auth]
    public class HomeController : Controller
    {
        public static Array GetMonths(DateTime date1, DateTime date2)
        {
            //Note - You may change the format of date as required.  
            return GetDates(date1, date2).ToArray();
        }

        public static IEnumerable<DateTime> GetDates(DateTime date1, DateTime date2)
        {
            while (date1 <= date2)
            {
                yield return date1;
                date1 = date1.AddMonths(1);
            }
            if (date1 > date2 && date1.Month == date2.Month)
            {
                // Include the last month  
                yield return date1;
            }
        }
        MyBudgetEntities db = new MyBudgetEntities();
        public ActionResult Index()
        {
            var currentDate = DateTime.Now;

            ViewBag.totalExpense = String.Format("{0:0.00}", db.Transaction.Where(x => x.UserId == CurrentSession.User.Id).Sum(x => x.Amount))  + " "+Defaults.MoneyType;
            ViewBag.totalThisMonthExpense = String.Format("{0:0.00}", db.Transaction.Where(x => x.UserId == CurrentSession.User.Id && x.CreatedDate.Value.Month == currentDate.Month && x.CreatedDate.Value.Year == currentDate.Year).Sum(x => x.Amount));
            ViewBag.totalThisMonthName = currentDate.ToString("MMMM");

            var dates = GetMonths(CurrentSession.User.CreatedDate.Value, currentDate);
            var islemler = db.Transaction.Where(x => x.UserId == CurrentSession.User.Id).ToList();

            //var totalMonth = db.Transaction.Where(x => x.CreatedDate.Value.Month == data.Month && x.CreatedDate.Value.Year == data.Year).Select(x => x.CreatedDate.Value.Month);

            List<DateTime> allDates = new List<DateTime>();
            List<int> allMonths = new List<int>();
            List<int> allYears = new List<int>();
            List<int> allAmount = new List<int>();

            foreach (DateTime item in dates)
            {
                allDates.Add(item);
                allMonths.Add(item.Month);
                allYears.Add(item.Year);
            }


            //var data = new TransactionList()
            //{
            //    Transaction = db.Transaction.Where(x => x.UserId == CurrentSession.User.Id).ToList()
            //};

            var data = db.Transaction.Where(x => x.UserId == CurrentSession.User.Id).ToList();




            ViewBag.allDates = allDates;
            ViewBag.allMonths = allMonths;
            ViewBag.years = allYears.Distinct().ToList();

            return View(data);
        }

    }
}