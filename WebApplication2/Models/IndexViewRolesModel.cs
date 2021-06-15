using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class IndexViewRolesModel
    {


        public List<Role> Roles { get; set; }
        public bool canAdd { get; set; }
        public bool canDelete { get; set; }
        public bool canEdit { get; set; }


    }
}