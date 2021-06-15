using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class CreateFileLevelModel
    {

        public CreateFileLevelModel()
        {
        }


        [Display(Name ="إسم المستوى")]
        public string levelName { get; set; }




        [Display(Name ="الوصف")]
        public string Desc { get; set; }

        public List<List<SelectListItem>>  Permissions { get; set; }
        public List<int> RolesIds { get; set; }
        public List<string> RolesNames { get; set; }


    }
}