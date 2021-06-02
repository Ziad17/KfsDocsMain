using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class ViewFileModel
    {
        public bool isBookmarked { get; set; }
        public File File { get; set; }
        public FileVersion CurrentVersion { get; set; }
        public List<FileVersion> Versions { get; set; }
        public EmployeeRole Owner { get; set; }
        public bool canEdit { get; set; }
        public bool hasCurrentVersion { get; set; }
        public bool canDelete { get; set; }
        public bool canAddVersion { get; set; }
        public bool canSetCurrentVersion { get; set; }
        public bool isAuthor { get; set; }




    }

}
