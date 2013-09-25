using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Data;

namespace SLSoft.ResidentProgram.Controllers
{
    public class OnLineChangeController : Controller
    {
        //
        // GET: /OnLineChange/

        public string Index()
        {
            string strJson = "";
            int SiteID = 0;
            int IntervalTime = 0;

            if (Request.QueryString["sId"] != null && Request.QueryString["iTime"] != null)
            {
                SiteID = int.Parse(Request.QueryString["sId"].ToString());
                IntervalTime = int.Parse(Request.QueryString["iTime"]);

                strJson = GetList(SiteID, IntervalTime);
            }
            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 获取最近XX分钟的PV、UV、IP变化情况
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        private string GetList(int SiteID, int IntervalTime)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", SiteID),
                new MySqlParameter("IntervalTime", IntervalTime)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_change", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        } 
    }
}
