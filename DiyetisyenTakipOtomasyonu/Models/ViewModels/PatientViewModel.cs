using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiyetisyenTakipOtomasyonu.Models.ViewModels
{
    public class PatientViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PatientPhoneNumber { get; set; }
        public int Weigth { get; set; }
        public int Heigth { get; set; }
        public int Endex { get; set; }
        public bool Gender { get; set; }
        public string PatientImage { get; set; }
        public int DoctorID { get; set; }
        public string Diagnostic { get; set; }
        public DateTime RandevuTarihi { get; set; }
    }
}