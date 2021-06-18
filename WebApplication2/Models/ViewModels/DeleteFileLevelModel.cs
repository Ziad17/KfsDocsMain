using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class DeleteFileLevelModel
    {

        public DeleteFileLevelModel()
        {
            RolesDirectory = new Dictionary<String, List<String>>();
        }

        public int ID { get; set; }


        public string Name { get; set; }
        public string Desc { get; set; }
        public Dictionary<String, List<String>> RolesDirectory { get; set; }
        public FileLevel Level { get; set; }
    }
}