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
        // 当前在线（最近XX分钟浏览次数、独立访客、IP数量）
        // GET: /OnLinePV/

        public string Index(string sId,string iTime)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(iTime))
            {
                strJson = GetInfo(int.Parse(sId), int.Parse(iTime));
            }
            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 获取最近XX分钟的浏览次数、独立访客、IP数量
        /// </summary>
        /// <param name="SiteID">站点ID</param>
        /// <param name="IntervalTime">最近多少分钟</param>
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
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_OnLinePV", mpara);

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