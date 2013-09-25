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
    public class OnLineMaxUVController : Controller
    {
        //
        // GET: /OnLineMaxUV/

        public string Index()
        {
            string strJson = "";
            int SiteID = 0;
            string callback = "";

            if (Request.QueryString["sId"] != null)
            {
                SiteID = int.Parse(Request.QueryString["sId"].ToString());

                strJson = GetInfo(SiteID);
            }
            callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 获取最近时间最高独立访客
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        private string GetInfo(int SiteID)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", SiteID)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_maxUV", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            else
            {
                strJson = "[{CNum:0,highNum:0,time:'00:00'}]";
            }
            return strJson;
        } 
    }
}
