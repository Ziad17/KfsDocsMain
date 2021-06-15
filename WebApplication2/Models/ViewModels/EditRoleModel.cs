using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class EditRoleModel
    {
        public int ID { get; set; }
        public List<SelectListItem> AvailablePersonPermissions { get; set; }
        public List<SelectListItem> AvailableInstitutionPermissions { get; set; }
        public string RoleName { get; set; }

    }
}