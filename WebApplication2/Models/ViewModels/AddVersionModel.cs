using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class AddVersionModel
    {




 
        public string InstitutionName { get; set; }

        public string RoleName { get; set; }




        public int FileID { get; set; }

        public string Number { get; set; }


        public string Notes { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file_content { get; set; }
        [DataType(DataType.DateTime)]
        public System.DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string AuthorName { get; set; }
    }
}