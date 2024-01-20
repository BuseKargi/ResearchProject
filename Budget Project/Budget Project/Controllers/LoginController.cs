using Budget_Project.Helpers;
using Budget_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget_Project.Controllers
{
    public class LoginController : Controller
    {
        MyBudgetEntities db = new MyBudgetEntities();
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(User model)
        {
            var data = db.User.SingleOrDefault(x => x.Email == model.Email);
            if (data == null) return View(model);

            var decryptPassword = Encrypt.DecryptPasswordBase64(data.Password);
            if (decryptPassword != model.Password) return View(model);

            if (data.IsVerified != true)
            {
                ModelState.AddModelError("NotVerified", "Lütfen Hesabınızı Doğrulayın!");
                return View();
            }
            CurrentSession.Set<User>("login", data);
            return RedirectToAction("Index", "Home");

        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var checkEMail = db.User.SingleOrDefault(x => x.Email == model.Email);

            if (checkEMail!=null)
            {
                ModelState.AddModelError("IsAlreadyEmail","Bu Mailde Kayıt Bulunmaktadır!");
                return View(model);
            }
            
            model.IsDelete = false;
            model.IsActive = true;
            model.CreatedDate = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            model.CreatedUser = "system";
            model.IsVerified = false;
            model.Password = Encrypt.EncryptPasswordBase64(model.Password);
            db.User.Add(model);
            db.SaveChanges();

            var lastUser = db.User.OrderByDescending(u => u.Id).FirstOrDefault();
            
            var siteUri = "https://localhost:44329/";
            if (lastUser == null) return RedirectToAction("Login", "Login");
            var activateUri = $"{siteUri}/Login/UserActivate/{lastUser.Id}";
            var body =
                $"Merhaba {model.Fullname};<br><br>Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";

            var checkSendMail = MailHelper.SendMail(body, model.Email, "Budget Hesap Aktifleştirme");
            if (checkSendMail)
            {
                return RedirectToAction("Login","Login");
            }
            ModelState.AddModelError("ErrorMailSend","Aktivasyon Maili Gönderilemedi, Bizimle İletişime Geçin.");
            return View(model);
        }
        
        public ActionResult UserActivate(int id)
        {
            var data = db.User.SingleOrDefault(x => x.Id==id);
            if (data == null) return RedirectToAction("Login");
            if (data.IsVerified == true) return RedirectToAction("Login");
            
            data.IsVerified = true;
            data.Fullname = "fuat";
            db.SaveChanges();
            return RedirectToAction("Login");
        }
        
        
        public ActionResult LogOut()
        {
            if (CurrentSession.User!=null)
            {
                CurrentSession.Clear();
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }
    }
}