using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SLSoft.EasyORM;
using System.Data;
using System.Data.SqlClient;

namespace SLSoft.ResidentProgram.Controllers
{
    public class StartController : Controller
    {
        //
        // GET: /Start/

        public ActionResult Index()
        {
            DBFactory factory = new DBFactory();

            string strdb = "";
            AbstractDB db = factory.CreateDB("mysql");
            DataTable dt = db.ExecSql("select * from slsoft_ias_sys_t_users");
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    strdb += row["UserName"].ToString()+"     ";
                }
            }
            ViewBag.strdb = strdb;
            return View();
        }

    }
}

