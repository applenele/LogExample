using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogExample.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            TraceLog.Operations.Append("增加用户");
            TraceLog.Operations.Append("增加接点");
            return View();
        }
    }
}