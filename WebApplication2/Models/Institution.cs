//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Institution
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Institution()
        {
            this.EmployeeRoles = new HashSet<EmployeeRole>();
            this.InstitutionActionLogs = new HashSet<InstitutionActionLog>();
            this.Institution1 = new HashSet<Institution>();
        }
    
        public int ID { get; set; }
        public string ArabicName { get; set; }
        public int InstitutionTypeID { get; set; }
        public bool Active { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string Website { get; set; }
        public string ImageURL { get; set; }
        public bool InsideCampus { get; set; }
        public string PrimaryPhone { get; set; }
        public string SecondaryPhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        public virtual InstitutionType InstitutionType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstitutionActionLog> InstitutionActionLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Institution> Institution1 { get; set; }
        public virtual Institution Institution2 { get; set; }
    }
}