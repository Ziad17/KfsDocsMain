using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels
{
    public class ViewFileLevelsModel
    {

        public ViewFileLevelsModel()
        {
            RolesDirectory=new Dictionary<String, List<String>>();
        }

        public Dictionary<String,List<String>> RolesDirectory { get; set; }
        public FileLevel Level { get; set; }
        public SelectList Levels { get; set; }

    }
}