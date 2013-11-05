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
    public class StatSourceByCodeController : Controller
    {
        // 搜索引擎（根据搜索引擎查看搜索词列表）
        // GET: /StatSourceByCode/

        public string Index(string sId,string sCode,string startDate,string endDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(sCode) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strJson = GetList(sId,sCode, startDate, endDate);
            }
            string callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 按来源引擎查看搜索词列表
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId,string sCode, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID",sId),                           
                new MySqlParameter("SourceCode", sCode),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59")
            };
            DataTable dt = null;

            if (startDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_SourceByCode", mpara);
            }
            else
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_his_SourceByCode", mpara);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }
    }
}