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
    public class StatSourceByCodeController : Controller
    {
        //
        // GET: /StatSourceByCode/

        public string Index()
        {
            string strJson = "";
            string sCode = "";
            string startDate = "";
            string endDate = "";
            string callback = "";

            if (Request.QueryString["sCode"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null)
            {
                sCode = Request.QueryString["sCode"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();

                strJson = GetList(sCode, startDate, endDate);
            }
            callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 按来源引擎查看搜索词列表
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sCode, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SourceCode", sCode),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59")
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_sourceByCode", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

    }
}
