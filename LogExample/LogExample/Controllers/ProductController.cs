using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LogExample.Controllers
{
    public class ProductController : ApiController
    {
        public string GetTest()
        {
            return "test";
        }



        public string PostTest()
        {
            int a = 0;
            int b = 10 / a;
            return "ok";
        }
    }
}
