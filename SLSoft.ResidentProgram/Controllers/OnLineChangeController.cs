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
        // 当前在线（最近xx分钟浏览次数、独立访客、IP数量趋势）
        // GET: /OnLineChange/

        public string Index(string sId, string iTime)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(iTime))
            {
                strJson = GetList(sId, iTime);
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
        private string GetList(string SiteID, string IntervalTime)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", SiteID),
                new MySqlParameter("IntervalTime", IntervalTime)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_OnLineChange", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        } 
    }
}
