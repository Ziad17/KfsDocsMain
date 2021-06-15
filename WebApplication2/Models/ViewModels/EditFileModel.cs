using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class EditFileModel
    {
        public string FileName { get; set; }
        public SelectList Levels { get; set; }
        public int LevelID { get; set; }
        public List<SelectListItem> AvailableEmployees { get; set; }
        public int ID { get; set; }
    }
}