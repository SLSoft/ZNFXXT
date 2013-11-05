using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;


namespace SLSoft.ResidentProgram.Controllers
{
    public class OnLineSessionController : Controller
    {
        // 当前在线（会话列表）
        // GET: /OnLineSession/

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
        /// 获取当前在线会话列表
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("siteID", sId),
                new MySqlParameter("startDate", startDate + " 00:00:00"),
                new MySqlParameter("endDate",endDate + " 23:59:59")
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_session", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = ToJson(dt);
            }
            return strJson;
        }

        #region 处理数据转换为json格式
        /// <summary>
        /// 处理数据转换为json格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ToJson(DataTable dt)
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
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append(string.Format("\"{0}\":", strKey));
                    strValue = String.Format(strValue, type);

                    if (strKey == "SourcePath" || strKey == "LastAccessPage")
                    {
                        strValue = Server.UrlEncode(strValue);
                    }

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
