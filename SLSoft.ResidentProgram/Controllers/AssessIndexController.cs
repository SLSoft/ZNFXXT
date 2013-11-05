using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Data;

namespace SLSoft.ResidentProgram.Controllers
{
    public class AssessIndexController : Controller
    {
        // 消费指数评估
        // GET: /AssessIndex/

        public string Index(string sId)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId))
            {
                strJson = GetList(sId);
            }
            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 根据站点ID获取消费指标评估数据
        /// </summary>
        /// <param name="sId"></param>
        /// <returns></returns>
        private string GetList(string sId)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId)
            };
            DataTable dt = db.ExecProcedureDateSet("slsoft_ias_bus_p_stat_AssessIndex", mpara).Tables[1];

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        } 

    }
}
