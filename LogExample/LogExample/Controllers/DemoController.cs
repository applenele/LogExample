using LogExample.Models;
using LogExample.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LogExample.Controllers
{
    public class DemoController : ApiController
    {
        [AllowAnonymous]
        public string PostParam([FromBody] vUser model)
        {
            return model.UserName.Length.ToString();
        }
    }
}
