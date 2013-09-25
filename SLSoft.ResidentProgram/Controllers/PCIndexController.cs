using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace SLSoft.ResidentProgram.Controllers
{
    public class PCIndexController : Controller
    {
        //
        // GET: /PCIndex/

        public string Index()
        {
            string strJson = "";
            string sId = "";
            string startDate = "";
            string endDate = "";
            string type = "";
            string callback = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();
                type = Request.QueryString["type"].ToString();

                strJson = GetList(sId, startDate, endDate, type);
            }
            callback = HttpContext.Request["callback"];
            return callback+"(" + strJson + ")";
        }

        /// <summary>
        /// 终端类型统计
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate, string type)
        {
            int otype = GetType(type);
            if (otype == 0)
            {
                return "";
            }

            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59"),
                new MySqlParameter("oType",otype)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_pcDeviceTypeIndex", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

        private static int GetType(string type)
        {
            switch (type.ToLower())
            {
                case "pv":
                    return 1;
                case "uv":
                    return 2;
                case "ip":
                    return 3;
                case "newuv":
                    return 4;
                case "count":
                    return 5;
            }
            return 0;
        }
    }
}
