using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels.InstitutionModels
{
    public class ViewInstitutionTypesModel
    {
        public List<InstitutionType> Types { get; set; }
        public bool canAdd { get; set; }
        public bool canEdit { get; set; }
        public bool canDelete { get; set; }

    }
}