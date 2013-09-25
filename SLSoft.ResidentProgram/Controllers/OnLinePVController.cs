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
    public class OnLinePVController : Controller
    {
        //
        // GET: /OnLinePV/

        public string Index()
        {
            string strJson = "";
            int SiteID = 0;
            int IntervalTime = 0;
            string callback = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["iTime"] != null)
            {
                SiteID = int.Parse(Request.QueryString["sId"].ToString());
                IntervalTime = int.Parse(Request.QueryString["iTime"]);

                strJson = GetInfo(SiteID,IntervalTime);
            }
            callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 获取最近XX分钟的浏览次数、独立访客、IP数量
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        private string GetInfo(int SiteID, int IntervalTime)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", SiteID),
                new MySqlParameter("IntervalTime", IntervalTime)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_PV", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            else
            {
                strJson = "[{PV:0,UV:0,IP:0}]";
            }
            return strJson;
        } 

    }
}
