using LogExample.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogExample.Controllers
{
    public class HomeController : BaseController
    {
        [RequireLog(OperateType = Models.OperateType.View)]
        public ActionResult Index()
        {
            TraceLog.Operations.Append("增加用户");
            TraceLog.Operations.Append("增加接点");

            TraceLog.Output = "true";
            return View();
        }
    }
}