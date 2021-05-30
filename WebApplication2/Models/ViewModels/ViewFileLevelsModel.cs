using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class ViewFileLevelsModel
    {

        public ViewFileLevelsModel()
        {
            LevelsDictionary=new Dictionary<String, Dictionary<String, List<String>>>();
        }

        public Dictionary<String,Dictionary<String,List<String>>> LevelsDictionary { get; set; }

    }
}