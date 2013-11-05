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
    public class StatAccessIndexController : Controller
    {
        // 受访分析--受访域名（按小时、天统计pv、count趋势图）
        // GET: /StatAccessIndex/

        static string startDate = "";
        static string endDate = "";

        public string Index()
        {
            string strJson = "";
            string sId = "";
            string type = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null && Request.QueryString["type"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();
                type = Request.QueryString["type"].ToString();

                strJson = GetList(sId, startDate, endDate, type);
            }
            string callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        #region 按受访页面统计 每小时、每天指标数
        /// <summary>
        /// 按受访页面统计 每小时、每天指标数
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate,string type)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59")
            };
            DataSet ds = null;
            if (startDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))//当天按小时统计
            {
                switch (type)
                {
                    case "pv":
                        ds = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_day_AccessHostPVHour", mpara);
                        break;
                    case "uv":
                        ds = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_day_AccessHostUVHour", mpara);
                        break;
                }
            }
            else//历史
            {
                if (startDate == endDate)//时间段为一天按小时统计
                {
                    switch (type)
                    {
                        case "pv":
                            ds = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_his_AccessHostPVHour", mpara);
                            break;
                        case "uv":
                            ds = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_his_AccessHostUVHour", mpara);
                            break;
                    }
                }
                else//按天统计
                {
                    switch (type)
                    {
                        case "pv":
                            ds = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_his_AccessHostPV", mpara);
                            break;
                        case "uv":
                            ds = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_his_AccessHostUV", mpara);
                            break;
                    }
                }
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                if (startDate == endDate)
                {
                    strJson = ToJson(ds);
                }
                else
                {
                    strJson = ToJson_day(ds);
                }
            }
            return strJson;
        }
        #endregion

        #region 将数据转换为定制的json格式
        public static string ToJson(DataSet ds)
        {
            DataTable dt1 = ds.Tables[0];
            DataTable dt2 = ds.Tables[1];

            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                jsonString.Append("{");
                DataRow[] rows = dt2.Select("path='"+dt1.Rows[i][0].ToString()+"'");
                jsonString.Append(string.Format("name:\"{0}\",data:[",dt1.Rows[i][0].ToString()));

                List<DataRow> list = new List<DataRow>();
                foreach (var item in rows)
                {
                    list.Add(item);
                }
                for (int j = 0; j < 24; j++)
                {
                    string strValue = "0";
                    for (int c = 0; c < list.Count; c++)
                    {
                        if (j == int.Parse(list[c][1].ToString()))
                        {
                            strValue = list[c][2].ToString();
                            list.Remove(list[c]);
                        }
                    }
                    jsonString.Append(strValue + ",");
                }
                jsonString.Remove(jsonString.Length - 1, 1);
               
                jsonString.Append("]");
                jsonString.Append("},");  
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        public static string ToJson_day(DataSet ds)
        {
            DataTable dt1 = ds.Tables[0];
            DataTable dt2 = ds.Tables[1];

            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                DataRow[] rows = dt2.Select("path='" + dt1.Rows[i][0].ToString() + "'");

                if (rows != null && rows.Length > 0)
                {
                    jsonString.Append("{");
                    jsonString.Append(string.Format("name:\"{0}\",data:[", dt1.Rows[i][0].ToString()));

                    List<DataRow> list = new List<DataRow>();
                    foreach (var item in rows)
                    {
                        list.Add(item);
                    }
                    List<string> days = GetDays();
                    for (int j = 0; j < days.Count; j++)
                    {
                        string strValue = "0";
                        for (int r = 0; r < rows.Length; r++)
                        {
                            if (days[j] == Convert.ToDateTime(rows[r][1].ToString()).ToString("yyyy-MM-dd"))
                            {
                                strValue = rows[r][2].ToString();
                                list.Remove(rows[r]);
                            }
                        }
                        jsonString.Append(strValue + ",");
                    }
                    jsonString.Remove(jsonString.Length - 1, 1);

                    jsonString.Append("]");
                    jsonString.Append("},");
                }
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        #endregion

        #region 获取时间段日期
        private static List<string> GetDays()
        {
            List<string> date = new List<string>();

            DateTime sDate = Convert.ToDateTime(startDate);
            DateTime eDate = Convert.ToDateTime(endDate);
            TimeSpan ts = eDate - sDate;

            for (int j = 0; j <= ts.Days; j++)
            {
                date.Add(sDate.AddDays(j).ToString("yyyy-MM-dd"));
            }
            return date;
        }
        #endregion
    }
}
