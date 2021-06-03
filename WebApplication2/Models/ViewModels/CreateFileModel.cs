using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class CreateFileModel
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string InstitutionName { get; set; }
        public string RoleName { get; set; }
        public List<int> MentionedIDs { get; set; }
        public string AuthorName { get; set; }
        public DateTime DateCreated { get; set; }


    }
}