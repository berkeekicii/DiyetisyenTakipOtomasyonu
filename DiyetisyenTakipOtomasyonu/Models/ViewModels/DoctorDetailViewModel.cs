using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiyetisyenTakipOtomasyonu.Models.ViewModels
{
    public class DoctorDetailViewModel
    {
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSurname { get; set; }
        public string DoctorEmail { get; set; }
        public string DoctorNumber { get; set; }
        public string DoctorPhoto { get; set; }
        public string DoctorCity{ get; set; }
        public string DoctorDistrict { get; set; }
    }
}