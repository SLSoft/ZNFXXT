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
    public class StatSourceCodeController : Controller
    {
        // 来源分析--搜索引擎统计
        // GET: /StatSourceCode/

        public string Index(string sId, string startDate, string endDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strJson = GetList(sId, startDate, endDate);
            }
            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 按搜索引擎统计
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate + " 00:00:00"),
                new MySqlParameter("endDate",endDate + " 23:59:59")
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_sourceCode", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

    }
}
