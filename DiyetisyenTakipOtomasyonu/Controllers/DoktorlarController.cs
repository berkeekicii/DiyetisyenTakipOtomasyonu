using DiyetisyenTakipOtomasyonu.Models;
using DiyetisyenTakipOtomasyonu.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    [AdminAuthorize]
    public class DoktorlarController : Controller
    {
        // GET: Doktorlar
        public async Task<ActionResult> Index(string searchQuery)
        {
            DiyetisyenTakipOtomasyonEntities2 context = new DiyetisyenTakipOtomasyonEntities2();
            List<Doctors> doktorlar;
            if (searchQuery != "" && searchQuery != null)
            {
                doktorlar = context.Doctors.Where(x => 
                    x.DoctorEmail.ToLower().Contains(searchQuery.ToLower()) || 
                    x.DoctorSurname.ToLower().Contains(searchQuery.ToLower()) || 
                    x.DoctorName.ToLower().Contains(searchQuery.ToLower()) || 
                    x.DoctorNumber.ToLower().Contains(searchQuery.ToLower())
                ).ToList();
            }
            else
            {
                doktorlar = context.Doctors.ToList();
            }


            List<DoctorDetailViewModel> doktorlarDetail = new List<DoctorDetailViewModel>();
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
            {
                foreach (var item in doktorlar)
                {
                    // Fall back to the stored codes if the city/district lookup fails,
                    // so a doctor never disappears from the list because of the API.
                    string provinceName = item.DoctorCity;
                    string districtName = item.DoctorDistrict;
                    try
                    {
                        var responseCity = await client.GetAsync($"https://turkiyeapi.dev/api/v1/provinces/{item.DoctorCity}");
                        var responseDistrict = await client.GetAsync($"https://turkiyeapi.dev/api/v1/districts/{item.DoctorDistrict}");
                        if (responseCity.IsSuccessStatusCode && responseDistrict.IsSuccessStatusCode)
                        {
                            dynamic city = JsonConvert.DeserializeObject(await responseCity.Content.ReadAsStringAsync());
                            dynamic district = JsonConvert.DeserializeObject(await responseDistrict.Content.ReadAsStringAsync());
                            provinceName = city.data.name;
                            districtName = district.data.name;
                        }
                    }
                    catch (Exception)
                    {
                        // keep the fallback values
                    }

                    doktorlarDetail.Add(new DoctorDetailViewModel
                    {
                        DoctorID = item.DoctorID,
                        DoctorCity = provinceName,
                        DoctorDistrict = districtName,
                        DoctorSurname = item.DoctorSurname,
                        DoctorEmail = item.DoctorEmail,
                        DoctorName = item.DoctorName,
                        DoctorNumber = item.DoctorNumber,
                        DoctorPhoto = item.DoctorPhoto,
                    });
                }
            }
            if (doktorlarDetail.Count() == 0)
            {
                return View();
            }
            else
            {
                return View(doktorlarDetail);
            }
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Doctors doktor, HttpPostedFileBase DoctorPhoto)
        {
            DiyetisyenTakipOtomasyonEntities2 context = new DiyetisyenTakipOtomasyonEntities2();

            Doctors doctors = new Doctors()
            {
                DoctorEmail = doktor.DoctorEmail,
                DoctorName = doktor.DoctorName,
                DoctorNumber = doktor.DoctorNumber,
                DoctorSurname = doktor.DoctorSurname,
                DoctorCity = doktor.DoctorCity,
                DoctorDistrict = doktor.DoctorDistrict,
            };

            if (DoctorPhoto != null && DoctorPhoto.ContentLength > 0)
            {
                var savedName = UploadHelper.Save(DoctorPhoto, Server);
                if (savedName != null) doctors.DoctorPhoto = savedName;
            }

            context.Doctors.Add(doctors);
            context.SaveChanges();
            return RedirectToAction("Index","Doktorlar");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var doctor = entities.Doctors.Include("Diagnosis").SingleOrDefault(p => p.DoctorID == id);

            if (doctor == null)
            {
                return HttpNotFound();
            }

            // A doctor who still has patients cannot be deleted (foreign key).
            if (entities.Patient.Any(p => p.DoctorID == id))
            {
                TempData["Error"] = "Bu doktora bağlı hastalar var. Önce hastaları başka bir doktora atayın.";
                return RedirectToAction("Index", "Doktorlar");
            }

            foreach (var diagnosis in doctor.Diagnosis.ToList())
            {
                entities.Diagnosis.Remove(diagnosis);
            }

            // Remove patient record
            entities.Doctors.Remove(doctor);

            entities.SaveChanges();

            return RedirectToAction("Index", "Doktorlar");
        }
        public async Task<ActionResult> Detail(int id) {
            
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var selectedDoktor = entities.Doctors.Where(x => x.DoctorID == id).FirstOrDefault();
            if (selectedDoktor == null) return HttpNotFound();

            string cityUrl = $"https://turkiyeapi.dev/api/v1/provinces/{selectedDoktor.DoctorCity}";
            string districtUrl = $"https://turkiyeapi.dev/api/v1/districts/{selectedDoktor.DoctorDistrict}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseCity = await client.GetAsync(cityUrl);
                HttpResponseMessage responseDistrict = await client.GetAsync(districtUrl);
                if (responseCity.IsSuccessStatusCode && responseDistrict.IsSuccessStatusCode)
                {
                    string provinceDetails = await responseCity.Content.ReadAsStringAsync();
                    string districtDetails = await responseDistrict.Content.ReadAsStringAsync();

                    dynamic responseObjectCity = Newtonsoft.Json.JsonConvert.DeserializeObject(provinceDetails);
                    dynamic responseObjectdistrict = Newtonsoft.Json.JsonConvert.DeserializeObject(districtDetails);

                    string provinceName = responseObjectCity.data.name;
                    string districtName = responseObjectdistrict.data.name;


                    ViewBag.ProvinceDetails = provinceName;
                    ViewBag.DistrictDetails = districtName;
                }
                else
                {
                    ViewBag.ProvinceDetails = "Error fetching province details";
                }
            }


            return View(selectedDoktor);


        }
        public ActionResult Update(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var selectedDoktor = entities.Doctors.Where(x => x.DoctorID == id).FirstOrDefault();
            if (selectedDoktor == null) return HttpNotFound();
            return View(selectedDoktor);
        }
        [HttpPost]
        public ActionResult Update(Doctors doc, HttpPostedFileBase DoctorPhoto) 
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var selectedDoktor = entities.Doctors.Where(x => x.DoctorID == doc.DoctorID).FirstOrDefault();
            if (selectedDoktor == null) return HttpNotFound();


            selectedDoktor.DoctorEmail = doc.DoctorEmail;
            selectedDoktor.DoctorSurname = doc.DoctorSurname;
            selectedDoktor.DoctorNumber = doc.DoctorNumber;
            selectedDoktor.DoctorName = doc.DoctorName;
            selectedDoktor.DoctorCity = doc.DoctorCity;
            selectedDoktor.DoctorDistrict = doc.DoctorDistrict;
            if (DoctorPhoto != null && DoctorPhoto.ContentLength > 0)
            {
                var savedName = UploadHelper.Save(DoctorPhoto, Server);
                if (savedName != null) selectedDoktor.DoctorPhoto = savedName;
            }

            entities.SaveChanges();
            return RedirectToAction("Index","Doktorlar");
        }
    }
}

namespace DiyetisyenTakipOtomasyonu.Controllers
{
    /// <summary>Saves uploaded images safely (extension whitelist, unique file name).</summary>
    public static class UploadHelper
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public static string Save(HttpPostedFileBase file, HttpServerUtilityBase server)
        {
            if (file == null || file.ContentLength <= 0) return null;

            var ext = Path.GetExtension(file.FileName ?? "").ToLowerInvariant();
            if (Array.IndexOf(Allowed, ext) < 0) return null;

            var dir = server.MapPath("~/Uploads");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var name = Guid.NewGuid().ToString("N") + ext;
            file.SaveAs(Path.Combine(dir, name));
            return name;
        }
    }
}
