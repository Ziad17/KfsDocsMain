using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class MenuModel
    {

        //employees
        public bool canCreateEmployee

        { get; set; }
     




        //institutions
        public bool canCreateInstitution { get; set; }

        //files
        public bool canCreateFile { get; set; }
        public bool canCreateLevel { get; set; }




        //roles
        public bool canViewAllRoles { get; set; }
        public bool canAttachRole { get; set; }
        public bool canCreateRole { get; set; }






        public int ID { get; set; }
        public string Img { get; set; }
        public string  Name { get; set; }
        public string  InstitutionName { get; set; }





    }
}