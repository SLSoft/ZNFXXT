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
    public class StatSourceIndexController : Controller
    {
        // 来源分类（统计小时/天 趋势图type指标为：pv，uv，ip，newuv，count）
        // GET: /StatSourceIndex/

        public string Index(string sId,string startDate,string endDate,string type)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(type))
            {
                strJson = GetList(sId, startDate, endDate, type);
            }
            string callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 按来源类别统计 每小时、每天指标数
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId, string startDate, string endDate,string type)
        {
            int otype = GetType(type);
            if (otype == 0){return "";}

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

            if (startDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))//当天
            {
                dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_SourceTrendHour", mpara);

                if (dt != null && dt.Rows.Count > 0)
                {
                    strJson = ToJson(dt);
                }
            }
            else //读历史数据
            {
                if (startDate == endDate) //按小时
                {
                    dt = db.ExecProcedure("slsoft_ias_bus_p_stat_his_SourceTrendHour",mpara);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        strJson = ToJson(dt);
                    }
                }
                else//按天数
                {
                    dt = db.ExecProcedure("slsoft_ias_bus_p_stat_his_SourceTrend", mpara);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        strJson = ToJson_day(dt);
                    }
                }
            }
            return strJson;
        }

        #region 将数据转换为定制的json格式

        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            
            for (int i = 1; i <= 4; i++)
            {
                jsonString.Append("{");
                DataRow[] rows = dt.Select(string.Format("sourceId={0}", i));
                jsonString.Append(string.Format("name:\"{0}\",data:[", SetSourceName(i)));

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

        public static string ToJson_day(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");

            for (int i = 1; i <= 4; i++)
            {
                jsonString.Append("{");
                DataRow[] rows = dt.Select(string.Format("sourceId={0}", i));
                jsonString.Append(string.Format("name:\"{0}\",data:[", SetSourceName(i)));

                string strValue = "";
                for (int r = 0; r < rows.Length; r++)
                {
                    strValue = rows[r][2].ToString();
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
        #endregion

        #region 自定义方法 返回相应的值

        /// <summary>
        /// 根据获取的指标参数返回对应的数值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据来源id设置来源名称
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static string SetSourceName(int i)
        {
            switch (i)
            {
                case 1:
                    return "直接输入网址或书签";
                case 2:
                    return "搜索引擎";
                case 3:
                    return "内部链接";
                case 4:
                    return "外部链接";
                default:
                    return "其它";
            }
        }
        #endregion
    }
}
