using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiyetisyenTakipOtomasyonu.Models.ViewModels
{
    public class PatientDetailViewModel
    {
        public int PatientID { get; set; }
        public string PatientNameSurname { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhoneNumber { get; set; }
        public int Weight { get; set; }
        public int Heigth { get; set; }
        public int Endex { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }
        public int DoctorID { get; set; }
        public string DoctorNameSurname { get; set; }
        public string DoctorEmail { get; set; }
        public string DoctorPhoneNumber { get; set; }
        public string DiagnosisValue { get; set; }
        public DateTime? RandevuTarihi { get; set; }

    }
}