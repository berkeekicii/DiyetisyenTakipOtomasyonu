//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiyetisyenTakipOtomasyonu.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Diagnosis
    {
        public int DiagnosisID { get; set; }
        public Nullable<int> DoctorID { get; set; }
        public Nullable<int> PatientID { get; set; }
        public string DiagnosisValue { get; set; }
    
        public virtual Doctors Doctors { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
