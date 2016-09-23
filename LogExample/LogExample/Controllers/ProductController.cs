using LogExample.Extensions;
using LogExample.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace LogExample.Controllers
{
    public class ProductController : ApiBaseController
    {
        [RequireLog]
        public string GetTest()
        {
            Operations.Append("Test");
            Operations.Append("Test Finish");
            Operations.AddToTraceLog(ActionContext);


            return "test";
        }


        public string GetParams(string name)
        {
            return name;
        }


        public string PostTest()
        {
            return "ok";
        }
    }
}
