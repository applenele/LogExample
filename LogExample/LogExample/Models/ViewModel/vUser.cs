using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogExample.Models.ViewModel
{
    public class vUser
    {
        public int Id { set; get; }

        [Required]
        public string UserName { set; get; }

        public string Password { set; get; }

        public string Email { set; get; }

        public string Phone { set; get; }

        public string Address { set; get; }
    }
}