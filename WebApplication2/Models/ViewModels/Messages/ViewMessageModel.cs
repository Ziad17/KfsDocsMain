using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels.Messages
{
    public class ViewMessageModel
    {

        public int ID { get; set; }
        public Employee Sender { get; set; }

        public Employee Reciever { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]

        public int RecieverID { get; set; }
        [Required(ErrorMessage = "This Field Is Required")]

        public int SenderID { get; set; }

        [Required(ErrorMessage = "This Field Is Required")]

        [Display(Name ="عنوان الرسالة")]
        [DataType(DataType.Text)]
        public string Header { get; set; }

        [Display(Name = "نص الرسالة")]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }


        [Required(ErrorMessage = "This Field Is Required")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase File { get; set; }


        public DateTime DateCreated { get; set; }

        public bool Sent { get; set; }

        public string TypeName { get; set; }
        public int Size { get; set; }


        public bool isAvailableForDelete()
        {
            if (Math.Abs(this.DateCreated.Subtract(DateTime.Now).TotalMinutes) < 15)
            {
                return true;
            }
            return false;
        }

    }
    
}