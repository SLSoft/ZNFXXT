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
    public class StatSourceCodeIndexController : Controller
    {
        //
        // GET: /StatSourceCodeIndex/
        static string startDate = "";
        static string endDate = "";

        public string Index()
        {
            string strJson = "";
            string sId = "";
            string type = "";
            string callback = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null && Request.QueryString["type"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();
                type = Request.QueryString["type"].ToString();

                strJson = GetList(sId, startDate, endDate, type);
            }
            callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 按搜索引擎统计 每小时、每天指标数
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate,string type)
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
            DataTable dt = null;
            if (startDate == endDate)
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_sourceCodeByHour", mpara);

                if (dt != null && dt.Rows.Count > 0)
                {
                    strJson = ToJson(dt);
                }
            }
            else
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_sourceCodeByDay",mpara);
                if (dt != null && dt.Rows.Count > 0)
                {
                    strJson = ToJson_day(dt);
                }
            }
            return strJson;
        }

        public static string ToJson(DataTable dt)
        {
            string[] strArray = GetSourceCode();
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");

            for (int i = 0; i <strArray.Length; i++)
            {
                DataRow[] rows = dt.Select("scode='"+strArray[i].ToLower()+"'");
                if (rows != null && rows.Length > 0)
                {
                    jsonString.Append("{");
                    jsonString.Append(string.Format("name:\"{0}\",data:[", rows[0][0].ToString()));

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
                    jsonString.Append("]},");
                }
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        public static string ToJson_day(DataTable dt)
        {
            string[] strArray = GetSourceCode();
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");

            for (int i = 0; i < strArray.Length; i++)
            {
                DataRow[] rows = dt.Select("scode='" + strArray[i].ToLower() + "'");
                if (rows != null && rows.Length > 0)
                {
                    jsonString.Append("{");
                    jsonString.Append(string.Format("name:\"{0}\",data:[", rows[0][0].ToString()));

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

        private static string[] GetSourceCode()
        {
            string[] strArray = new string[] { "Baidu", "Google","yisou", "MSN", "Yahoo", "live", "tom"
            , "163", "TMCrawler", "iask", "Sogou", "soso", "youdao", "zhongsou", "3721", "openfind",
            "alltheweb", "lycos", "bing", "118114","360" };

            return strArray;
        }
    }
}
