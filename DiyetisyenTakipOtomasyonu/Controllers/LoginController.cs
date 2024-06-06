using DiyetisyenTakipOtomasyonu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    public class LoginController : Controller
    {
        

        public ActionResult Index(string Email, string pass)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var admin = entities.Admin.Where(x => x.Email == Email && x.Password == pass).FirstOrDefault();
            if (admin != null) 
            {
                return RedirectToAction("Index","Home", new {adminID = admin.UserID});
            }
            else
            {
                return View();
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Admin newAdmin)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();
            Admin admin = new Admin()
            {
                Email = newAdmin.Email,
                Password = newAdmin.Password,
                Name = newAdmin.Name,
                Surname = newAdmin.Surname,
            };

            entities.Admin.Add(admin);
            entities.SaveChanges();
            return RedirectToAction("Index", "Login");
        }
    }
}