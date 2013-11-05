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
    public class StatAccessByHostController : Controller
    {
        // 受访分析--受访域名明细
        // GET: /StatAccessByHost/

        public string Index(string sId,string sPath,string startDate, string endDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(sPath) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strJson = GetList(sId,sPath, startDate, endDate);
            }
            string callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 根据受访域名查看明细
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId,string sPath, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59"),
                new MySqlParameter("HostPath",sPath)
            };
            DataTable dt = null;

            if (startDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_AccessPathByHost", mpara);
            }
            else
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_his_AccessPathByHost", mpara);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }
    }
}
