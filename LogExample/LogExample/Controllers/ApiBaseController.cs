using LogExample.Extensions;
using LogExample.Models;
using LogExample.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Threading;
using System.Threading.Tasks;

namespace LogExample.Controllers
{
    public class ApiBaseController : ApiController
    {
        public User CurrentUser { set; get; }

        public StringBuilder Operations { set; get; }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            Operations = new StringBuilder();
            CurrentUser = HttpContext.Current.Session.GetCurrentUserInfo();
            base.Initialize(controllerContext);
        }



    }
}
