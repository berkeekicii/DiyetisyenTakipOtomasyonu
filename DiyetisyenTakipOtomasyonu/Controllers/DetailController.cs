using DiyetisyenTakipOtomasyonu.Models;
using DiyetisyenTakipOtomasyonu.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    public class DetailController : Controller
    {
        // GET: Detail
        public ActionResult Index(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();
            var patient = (from pt in entities.Patient
                           join dg in entities.Diagnosis on pt.PatientID equals dg.PatientID
                           join dt in entities.Doctors on pt.DoctorID equals dt.DoctorID
                           where pt.PatientID == id
                           select new PatientDetailViewModel
                           {
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
                               RandevuTarihi = pt.RandevuTarihi
                           }).FirstOrDefault();
            return View(patient);
        }
        public ActionResult DoctorDetail(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();
            var doctor =  entities.Doctors.FirstOrDefault(x => x.DoctorID == id);
            return View(doctor);
        }
    }
}