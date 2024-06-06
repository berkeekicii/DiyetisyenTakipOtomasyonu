using DiyetisyenTakipOtomasyonu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult Index(string searchQuery)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            if (searchQuery == null)
            {
                var hastalarım = entities.Patient.ToList();
                return View(hastalarım);
            }
            else
            {
                var hastalarım = entities.Patient
                    .Where(p => p.PatientName.ToLower().Contains(searchQuery.ToLower()) ||
                                p.PatientSurname.ToLower().Contains(searchQuery.ToLower()) ||
                                (p.Weigth.Value.ToString().ToLower() == searchQuery.ToLower()) ||
                                (p.Heigth.Value.ToString().ToLower() == searchQuery.ToLower()) ||
                                (p.Endex.Value.ToString().ToLower() == searchQuery.ToLower()))
                    .ToList();
                return View(hastalarım);
            }
        
        }
      
        public ActionResult Doctors()
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var doctors = entities.Doctors.ToList();
            return View(doctors);
        }

        public ActionResult Remove(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var patient = entities.Patient.Include("Diagnosis").SingleOrDefault(p => p.PatientID == id);

            if (patient == null)
            {
                return HttpNotFound();
            }

            // Remove related diagnosis records (if any)
            foreach (var diagnosis in patient.Diagnosis.ToList())
            {
                entities.Diagnosis.Remove(diagnosis);
            }

            // Remove patient record
            entities.Patient.Remove(patient);

            entities.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}