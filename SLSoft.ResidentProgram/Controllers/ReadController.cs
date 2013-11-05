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
    public class ReadController : Controller
    {
        // 趋势分析（获取指标）
        // GET: /Read/

        DBFactory df = new DBFactory();
        AbstractDB db = null;
        
        public string Index(string sId,string startDate,string endDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                strJson = GetList(sId, startDate, endDate);
            }
            string callback = HttpContext.Request["callback"];
            return callback+"(" + strJson + ")";
        }

        /// <summary>
        /// 统计指标
        /// </summary>
        /// <param name="sId">站点ID</param>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate)
        {
            string strJson = "";
            DataTable dt = null;

            if (startDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))
            {
                dt = GetTodayList(sId, startDate, endDate);
            }
            else
            {
                dt = GetHistoryList(sId, startDate, endDate);
            }
            
            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

        #region 获取指标数据
        /// <summary>
        /// 趋势分析（获取当天指标）
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private DataTable GetTodayList(string sId, string startDate, string endDate)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59")
            };
            DataTable dt = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_day_TrendIndex", mpara).Tables[1];
            
            return dt;
        }

        /// <summary>
        /// 趋势分析（获取历史指标）
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private DataTable GetHistoryList(string sId, string startDate, string endDate)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate),
                new MySqlParameter("endDate",endDate)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_his_TrendIndex", mpara);

            return dt;
        }

        #endregion
    }
}
