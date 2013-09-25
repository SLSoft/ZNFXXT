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
        //
        // GET: /TrendByDay/

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
            callback = HttpContext.Request["callback"];
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
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_flowanalysisByDay", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = ToJson(dt);
            }
            return strJson;
        }


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
    }
}
