using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Text;

namespace SLSoft.ResidentProgram.Controllers
{
    public class TrendByDayController : Controller
    {
        // 趋势分析（按天统计）
        // GET: /TrendByDay/

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
        /// 按天统计
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
                new MySqlParameter("startDate", startDate),
                new MySqlParameter("endDate",endDate)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_Trend", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = ToJson(dt);
            }
            return strJson;
        }

        #region 数据处理为json格式
        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = "";
                    if (j == 0)
                    {
                        DateTime date = DateTime.Parse(drc[i][0].ToString()).Date;
                        strValue = date.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        strValue = drc[i][j].ToString();
                    }
                    
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append(string.Format("\"{0}\":", strKey));
                    strValue = String.Format(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(string.Format("\"{0}\",", strValue));
                    }
                    else
                    {
                        jsonString.Append(string.Format("\"{0}\"", strValue));
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        #endregion
    }
}
