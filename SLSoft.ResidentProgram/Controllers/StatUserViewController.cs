using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;

namespace SLSoft.ResidentProgram.Controllers
{
    public class StatUserViewController : Controller
    {
        //
        // GET: /StatUserView/

        public string Index()
        {
            string strJson = "";
            string sId = "";
            string path = "";
            string date = "";
            string callback = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["path"] != null && Request.QueryString["date"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                path = Request.QueryString["path"].ToString();
                date = Request.QueryString["date"].ToString();

                strJson = GetList(sId, path, date);
            }
            callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }


        private string GetList(string sId, string path, string date)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("Path", path),
                new MySqlParameter("Date",date)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_userView", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }
    }
}
