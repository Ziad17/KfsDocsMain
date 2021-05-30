using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class SendMessageModel
    {


        public Employee Sender { get; set; }
        
        public Employee Reciever { get; set; }

        [Required(ErrorMessage ="This Field Is Required")]

        public int RecieverID { get; set; }
        [Required(ErrorMessage = "This Field Is Required")]

        public int SenderID { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]

        [Display(Name  ="نص الرسالة")]
        [DataType(DataType.Text)]
        public string Header { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text{ get; set; }


        [Required(ErrorMessage = "This Field Is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }

    }
}