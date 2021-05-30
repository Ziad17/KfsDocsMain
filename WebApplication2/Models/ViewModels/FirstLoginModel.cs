using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models.ViewModels
{
    public class FirstLoginModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage ="This Field Is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage ="This Field Is Required")]
        [MinLength(length:16,ErrorMessage ="The Acadmic Number Must Be 16 Digits")]
        [MaxLength(length: 16, ErrorMessage = "The Acadmic Number Must Be 16 Digits")]
        public string AcadmicNumber { get; set; }
        [Required(ErrorMessage = "This Field Is Required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        [MinLength(length:8,ErrorMessage ="The Password Must Be At Least 8 Characters Or Digits")]
        [Required(ErrorMessage = "This Field Is Required")]
        [DataType(DataType.Password)]
        public string  ConfirmPassword { get; set; }
    }
}