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
    public class ColumnPVIndexController : Controller
    {
        // 按栏目浏览量统计（一级栏目）
        // GET: /ColumnPVIndex/

        public string Index(string startDate, string endDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strJson = GetList(startDate, endDate);
            }

            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        private string GetList(string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("f_ID","0"),                           
                new MySqlParameter("startDate", startDate),
                new MySqlParameter("endDate",endDate)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_ColumnPV", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        } 
    }
}
