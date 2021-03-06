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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.EmployeeCredentials = new HashSet<EmployeeCredential>();
            this.EmployeeRoles = new HashSet<EmployeeRole>();
            this.FileMentions = new HashSet<FileMention>();
            this.Messages = new HashSet<Message>();
            this.Messages1 = new HashSet<Message>();
            this.PersonActionLogs = new HashSet<PersonActionLog>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public bool Active { get; set; }
        public string AcadmicNumber { get; set; }
        public int CityID { get; set; }
        public Nullable<int> PrimaryRoleID { get; set; }
        public string PhoneNumber { get; set; }
        public string PHD { get; set; }
        public string ImageURL { get; set; }
        public string Bio { get; set; }
    
        public virtual City City { get; set; }
        public virtual EmployeeRole EmployeeRole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeCredential> EmployeeCredentials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FileMention> FileMentions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Messages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Messages1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonActionLog> PersonActionLogs { get; set; }
    }
}
