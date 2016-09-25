using LogExample.Extensions;
using LogExample.Helper;
using LogExample.Models;
using LogExample.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LogExample.Controllers
{
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [RequireLog(OperateType = OperateType.View)]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireLog(OperateType = OperateType.Action)]
        public ActionResult Login(string UserName, string Password)
        {
            using (DB db = new DB())
            {

                Password = Password.ToMD5Hash();
                User user = db.Users.Where(x => x.UserName == UserName && x.Password == Password).FirstOrDefault();
                if (user != null)
                {
                    Session.PutUserIDInSession(EncryptHelper.DESEncode(user.Id.ToString()));
                    return Redirect("/Home/Index");
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult GetJson()
        {
            return Json("sss");
        }
    }
}