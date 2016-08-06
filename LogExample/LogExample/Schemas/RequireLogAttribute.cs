using LogExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogExample.Schemas
{
    public class RequireLogAttribute : Attribute
    {
        public OperateType OperateType { set; get; }
    }
}