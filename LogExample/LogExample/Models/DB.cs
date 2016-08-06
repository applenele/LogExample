using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LogExample.Models
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class DB : DbContext
    {

        public DB() : base("name=mysqldb")
        {
        }


        public DbSet<User> Users { set; get; }

        public DbSet<OperateLog> OperateLogs { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}