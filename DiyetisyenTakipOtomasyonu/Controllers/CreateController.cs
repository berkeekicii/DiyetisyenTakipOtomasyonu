using DiyetisyenTakipOtomasyonu.Models;
using DiyetisyenTakipOtomasyonu.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    public class CreateController : Controller
    {
        // GET: Create
        public ActionResult Index()
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var doctors = entities.Doctors.ToList();
            return View(doctors);
        }
        [HttpPost]
        public ActionResult Create(PatientViewModel patient , HttpPostedFileBase PatientImage) 
        {
            if (PatientImage != null && PatientImage.ContentLength > 0)
            {
                // Ensure the Uploads directory exists
                var uploadDir = "~/Uploads";
                var physicalUploadDir = Server.MapPath(uploadDir);

                if (!Directory.Exists(physicalUploadDir))
                {
                    Directory.CreateDirectory(physicalUploadDir);
                }

                // Generate a unique filename
                var fileName = Path.GetFileName(PatientImage.FileName);
                var path = Path.Combine(physicalUploadDir, fileName);

                // Save the file to the server
                PatientImage.SaveAs(path);

                // Save the file path or name to the database
                patient.PatientImage = fileName;
            }

            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();
            var newPatient = new Patient
            {
                DoctorID = patient.DoctorID,
                Gender = patient.Gender,
                Endex = patient.Endex,
                Heigth = patient.Heigth,
                PatientEmail = patient.Email,
                PatientName = patient.Name,
                PatientImage = patient.PatientImage,
                PatientPhoneNumber = patient.PatientPhoneNumber,
                PatientSurname = patient.Surname,
                Weigth = patient.Weigth,
                RandevuTarihi = patient.RandevuTarihi
            };
            entities.Patient.Add(newPatient);
            entities.SaveChanges();
            var diagnostic = new Diagnosis
            {
                DiagnosisValue = patient.Diagnostic,
                DoctorID = newPatient.DoctorID,
                PatientID = newPatient.PatientID
            };
            entities.Diagnosis.Add(diagnostic);
            entities.SaveChanges();



            return RedirectToAction("Index","Home");
        }


        public ActionResult Update(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            ViewBag.doctorsUpdate = entities.Doctors.ToList();

            var selected = (from pt in entities.Patient
                           join dg in entities.Diagnosis on pt.PatientID equals dg.PatientID
                           join dt in entities.Doctors on pt.DoctorID equals dt.DoctorID
                           where pt.PatientID == id
                           select new PatientDetailViewModel
                           {
                               PatientID = pt.PatientID,
                               DoctorEmail = dt.DoctorEmail,
                               DoctorNameSurname = dt.DoctorName + " " + dt.DoctorSurname,
                               DoctorPhoneNumber = dt.DoctorNumber,
                               Endex = (int)pt.Endex,
                               Gender = (pt.Gender == false ? "Erkek" : "Kadın"),
                               Heigth = (int)pt.Heigth,
                               Image = pt.PatientImage,
                               PatientEmail = pt.PatientEmail,
                               PatientNameSurname = pt.PatientName + " " + pt.PatientSurname,
                               PatientPhoneNumber = pt.PatientPhoneNumber,
                               Weight = (int)pt.Weigth,
                               DiagnosisValue = dg.DiagnosisValue,
                           }).FirstOrDefault(); 
            return View(selected);
        }

        [HttpPost]
        public ActionResult Update(PatientDetailViewModel patient, HttpPostedFileBase PatientImage)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();
            var selected = entities.Patient.Where(x => x.PatientID == patient.PatientID).FirstOrDefault();

            selected.Weigth = patient.Weight;
            selected.PatientPhoneNumber = patient.PatientPhoneNumber;
            selected.PatientSurname = patient.PatientNameSurname.Split(' ').Last();
            selected.PatientName = patient.PatientNameSurname.Split(' ').First();
            selected.PatientEmail = patient.PatientEmail;
            selected.DoctorID = patient.DoctorID;
            selected.Gender = (patient.Gender == "Erkek" ? false : true);
            selected.Heigth = patient.Heigth;

            if (PatientImage != null && PatientImage.ContentLength > 0)
            {
                var uploadDir = "~/Uploads";
                var physicalUploadDir = Server.MapPath(uploadDir);

                if (!Directory.Exists(physicalUploadDir))
                {
                    Directory.CreateDirectory(physicalUploadDir);
                }

                var fileName = Path.GetFileName(PatientImage.FileName);
                var path = Path.Combine(physicalUploadDir, fileName);

                PatientImage.SaveAs(path);

                selected.PatientImage = fileName;
            }


            var diagnosis = entities.Diagnosis.FirstOrDefault(x => x.PatientID == selected.PatientID);
            diagnosis.DiagnosisValue = patient.DiagnosisValue;
            diagnosis.DoctorID = selected.DoctorID;



            entities.SaveChanges();

            return RedirectToAction("Index","Home");

        }
    }
}