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
    public class ReadController : Controller
    {
        //
        // GET: /Read/

        public ActionResult Index()
        {
            string strJson = "";
            string sId = "";
            string startDate = "";
            string endDate = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();

                strJson = GetStatisticalTarget(sId,startDate,endDate,"");
            }

            ViewBag.json = strJson;
            return View();
        }

        /// <summary>
        /// 统计指标
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
                new MySqlParameter("StatisticsSite_ID", sId),
                new MySqlParameter("startDate", startDate),
                new MySqlParameter("endDate",endDate)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_flowanalysis", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = ToJson(dt);
            }
            return strJson;
        }


        #region Datatable转换为Json 
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
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
                    string strValue = drc[i][j].ToString(); 
                    Type type = dt.Columns[j].DataType; 
                    jsonString.Append("\"" + strKey + "\":"); 
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
        /// DataTable转换为Json 
        /// </summary> 
        public static string ToJson(DataTable dt, string jsonName) 
        { 
            StringBuilder Json = new StringBuilder(); 
            if (string.IsNullOrEmpty(jsonName)) jsonName = dt.TableName; 
            Json.Append("{\"" + jsonName + "\":["); 
            if (dt.Rows.Count > 0) 
            { 
                for (int i = 0; i < dt.Rows.Count; i++) 
                { 
                    Json.Append("{"); 
                    for (int j = 0; j < dt.Columns.Count; j++) 
                    { 
                        Type type = dt.Rows[i][j].GetType(); 
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + String.Format(dt.Rows[i][j].ToString(), type)); 
                        if (j < dt.Columns.Count - 1) 
                        { 
                            Json.Append(","); 
                        } 
                    } 
                    Json.Append("}"); 
                    if (i < dt.Rows.Count - 1) 
                    { 
                        Json.Append(","); 
                    } 
                } 
            } 
            Json.Append("]}"); 
            return Json.ToString(); 
        } 
        #endregion 


    }
}
