using LogExample.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogExample.Controllers
{
    public class ErrorController : Controller
    {
        [ValidateInput(false)]
        public ActionResult Path500(ErrorMessage msg)
        {
            ViewData = new ViewDataDictionary<ErrorMessage>(msg);
            return View("ISE");
        }
    }
}