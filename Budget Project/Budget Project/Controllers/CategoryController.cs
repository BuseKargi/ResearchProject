using Budget_Project.Filters;
using Budget_Project.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Budget_Project.Controllers
{
    [Auth]
    public class CategoryController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();

        // GET: Category
        public ActionResult Index()
        {
            var currentUser = CurrentSession.User.Id;
            var category = db.Category.Where(x => x.UserId == currentUser).ToList();
            return View(category);
        }

        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(),
                "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category model, HttpPostedFileBase LogoPath)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ParentId =
                    new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id",
                        "Name");

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

        public ActionResult Edit(int? id)
        {
            List<SelectListItem> Status = new List<SelectListItem>();
            Status.Add(new SelectListItem() { Text = "Income", Value = "false" });
            Status.Add(new SelectListItem() { Text = "Expense", Value = "true" });


            Category category = db.Category.Find(id);
            ViewBag.IsStatus = new SelectList(Status, "Value", "Text");

            var selectedSubCategory = db.Category.SingleOrDefault(x => x.Id == category.ParentId);

            if (selectedSubCategory != null)
            {
                ViewBag.ParentId =
                    new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id", "Name",
                        selectedSubCategory.Id);
            }
            else
            {
                if (category.ParentId == 0)
                {
                    ViewBag.ParentId =
                        new SelectList(
                            db.Category.Where(x => x.ParentId == 0 && x.IsActive == true && x.Id != category.Id)
                                .ToList(), "Id", "Name");
                }
                else
                {
                    ViewBag.ParentId =
                        new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id",
                            "Name");
                }
            }

            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(Category model, HttpPostedFileBase LogoPath)
        {
            if (!ModelState.IsValid) return View(model);

            var data = db.Category.SingleOrDefault(x => x.Id == model.Id);

            if (LogoPath != null)
            {
                string _LogoPath = $"{model.Name}.{LogoPath.ContentType.Split('/')[1]}";
                LogoPath.SaveAs(Server.MapPath($"~/Assets/images/category/{_LogoPath}"));
                data.Logo = _LogoPath;
            }
            else
            {
                model.Logo = null;
            }

            var selectedSubCategory = db.Category.SingleOrDefault(x => x.Id == data.ParentId);
            if (selectedSubCategory != null)
            {
                ViewBag.ParentId =
                    new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id", "Name",
                        selectedSubCategory.Id);
            }
            else
            {
                ViewBag.ParentId =
                    new SelectList(db.Category.Where(x => x.ParentId == 0 && x.IsActive == true).ToList(), "Id",
                        "Name");
            }

            data.ParentId = model.ParentId == null ? 0 : model.ParentId;
            data.Name = model.Name;
            data.IsActive = model.IsActive;
            data.ModifiedDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            if (id != null)
            {
                Transaction trs = db.Transaction.FirstOrDefault(x => x.CategoryId == id);
                if (trs == null)
                {
                    Category data = db.Category.FirstOrDefault(x => x.Id == id);
                    if (data != null) //eğer bu kategoriyi kullanan transaction yok ise silmesine izin ver
                    {
                        db.Category.Remove(data);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                else //eğer bu kategoriyi kullanan transaction var ise silmesine izin verme tekrar döndür.
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }
    }
}