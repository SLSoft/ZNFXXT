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
    public class StatSourceByHostPathController : Controller
    {
        // 来源分析--来路域名明细列表
        // GET: /StatSourceByHostPath/

        public string Index(string sId,string sPath,string startDate,string endDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(sPath) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strJson = GetList(sId, sPath, startDate, endDate);
            }
            string callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 按来路域名查询明细列表
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="sPath"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private string GetList(string sId,string sPath, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("HostPath", sPath),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59")
            };
            DataTable dt = null;

            if (startDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_SourcePathByHost", mpara);
            }
            else
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_his_SourcePathByHost", mpara);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }
    }
}