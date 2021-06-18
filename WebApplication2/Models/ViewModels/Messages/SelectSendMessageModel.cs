using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Models.ViewModels.Messages
{
    public class SelectSendMessageModel
    {
     


        public Employee Sender { get; set; }



        public List<SelectListItem> Employees { get; set; }
        public int RecieverID { get; set; }

        public int SenderID { get; set; }

        [Required(ErrorMessage = "هذا الحقل مطلوب")]

        [Display(Name = "العنوان")]
        [DataType(DataType.Text)]
        public string Header { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "نص الرسالة")]

        public string Text { get; set; }


        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }

    }
}