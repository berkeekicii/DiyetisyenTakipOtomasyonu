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
            foreach (var item in doktorlar)
            {


                

                string cityUrl = $"https://turkiyeapi.dev/api/v1/provinces/{item.DoctorCity}";
                string districtUrl = $"https://turkiyeapi.dev/api/v1/districts/{item.DoctorDistrict}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage responseCity = await client.GetAsync(cityUrl);
                    HttpResponseMessage responseDistrict = await client.GetAsync(districtUrl);
                    if (responseCity.IsSuccessStatusCode && responseDistrict.IsSuccessStatusCode)
                    {
                        string provinceDetails = await responseCity.Content.ReadAsStringAsync();
                        string districtDetails = await responseDistrict.Content.ReadAsStringAsync();

                        dynamic responseObjectCity = JsonConvert.DeserializeObject(provinceDetails);
                        dynamic responseObjectDistrict = JsonConvert.DeserializeObject(districtDetails);

                        string provinceName = responseObjectCity.data.name;
                        string districtName = responseObjectDistrict.data.name;


                        ViewBag.ProvinceDetails = provinceName;
                        ViewBag.DistrictDetails = districtName;
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
                    else
                    {
                        ViewBag.ProvinceDetails = "Error fetching province details";
                    }
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
                var uploadDir = "~/Uploads";
                var physicalUploadDir = Server.MapPath(uploadDir);

                if (!Directory.Exists(physicalUploadDir))
                {
                    Directory.CreateDirectory(physicalUploadDir);
                }

                var fileName = Path.GetFileName(DoctorPhoto.FileName);
                var path = Path.Combine(physicalUploadDir, fileName);

                DoctorPhoto.SaveAs(path);

                doctors.DoctorPhoto = fileName;
            }

            context.Doctors.Add(doctors);
            context.SaveChanges();
            return RedirectToAction("Index","Doktorlar");
        }
        public ActionResult Remove(int id)
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var doctor = entities.Doctors.Include("Diagnosis").SingleOrDefault(p => p.DoctorID == id);

            if (doctor == null)
            {
                return HttpNotFound();
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
            return View(selectedDoktor);
        }
        [HttpPost]
        public ActionResult Update(Doctors doc, HttpPostedFileBase DoctorPhoto) 
        {
            DiyetisyenTakipOtomasyonEntities2 entities = new DiyetisyenTakipOtomasyonEntities2();

            var selectedDoktor = entities.Doctors.Where(x => x.DoctorID == doc.DoctorID).FirstOrDefault();


            selectedDoktor.DoctorEmail = doc.DoctorEmail;
            selectedDoktor.DoctorSurname = doc.DoctorSurname;
            selectedDoktor.DoctorNumber = doc.DoctorNumber;
            selectedDoktor.DoctorName = doc.DoctorName;
            selectedDoktor.DoctorCity = doc.DoctorCity;
            selectedDoktor.DoctorDistrict = doc.DoctorDistrict;
            if (DoctorPhoto != null && DoctorPhoto.ContentLength > 0)
            {
                var uploadDir = "~/Uploads";
                var physicalUploadDir = Server.MapPath(uploadDir);

                if (!Directory.Exists(physicalUploadDir))
                {
                    Directory.CreateDirectory(physicalUploadDir);
                }

                var fileName = Path.GetFileName(DoctorPhoto.FileName);
                var path = Path.Combine(physicalUploadDir, fileName);

                DoctorPhoto.SaveAs(path);

                selectedDoktor.DoctorPhoto = fileName;
            }

            entities.SaveChanges();
            return RedirectToAction("Index","Doktorlar");
        }
    }
}