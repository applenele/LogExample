using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogExample.Models
{
    public class User
    {
        public int Id { set; get; }

        public string UserName { set; get; }

        public string Password { set; get; }

        public string Email { set; get; }

        public string Phone { set; get; }

        public string Address { set; get; }

        public Role Role { set; get; }
    }

    public enum Role
    {
        Admin,
        Operator
    }
}