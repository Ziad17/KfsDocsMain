using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class CreateFileModel
    {
        public string Name { get; set; }
        public SelectList Levels { get; set; }
        public int LevelID { get; set; }
        public string InstitutionName { get; set; }
        public string RoleName { get; set; }
        public string AuthorName { get; set; }
        public DateTime DateCreated { get; set; }
        public List<SelectListItem> AvailableEmployees { get; set; }

        public int RoleID { get; set; }
        public int InstitutionID { get; set; }



    }
}