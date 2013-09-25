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
    public class TrendByHourController : Controller
    {
        //
        // GET: /TrendByHour/

        public string Index()
        {
            string strJson = "";
            string sId = "";
            string startDate = "";
            string endDate = "";
            string callback = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();

                strJson = GetStatisticalTarget(sId, startDate, endDate, "");
            }
            callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 时间段统计
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private string GetStatisticalTarget(string sId, string startDate, string endDate, string token)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate),
                new MySqlParameter("endDate",endDate)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_flowanalysisByHour", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < 24; i++)
            {
                jsonString.Append("{");

                DataRow[] row = dt.Select(string.Format("hour={0}", i));

                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = "";
                    if (j == 0)
                    {
                        strValue = string.Format("\"{0}\"", GetHours(i));
                    }
                    else
                    {
                        if (row.Length > 0)
                        {
                            strValue = string.Format("\"{0}\"",row[0][j].ToString());
                        }
                        else
                        {
                            strValue = strKey == "hour" ? (i + 1).ToString() : string.Format("\"{0}\"","-");
                        }
                    }
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append(string.Format("\"{0}\":", strKey));
                    strValue = String.Format(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        /// <summary>
        /// 获取时间段
        /// </summary>
        /// <returns></returns>
        private static string GetHours(int i)
        {
            if (i < 10)
            {
                return string.Format("0{0}:00-0{0}:59", i);
            }
            else
            {
                return string.Format("{0}:00-{0}:59", i);
            }
        }
    }
}
