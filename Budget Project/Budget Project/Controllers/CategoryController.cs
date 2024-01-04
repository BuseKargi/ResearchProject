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
        [Auth]
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category model, HttpPostedFileBase LogoPath)
        {

            if (ModelState.IsValid)
            {
                ViewBag.ParentId = new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id", "Name");

                if (LogoPath != null)
                {
                    string _LogoPath = $"{model.Name}.{LogoPath.ContentType.Split('/')[1]}";
                    LogoPath.SaveAs(Server.MapPath($"~/Assets/images/category/{_LogoPath}"));
                    model.Logo = _LogoPath;
                }
                else
                {
                    model.Logo = null;
                }

                var currentUser = CurrentSession.User.Id;

                model.ParentId = model.ParentId == null ? 0 : model.ParentId;
                model.UserId = currentUser;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                db.Category.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}