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
    public class StatSourceByHostPathController : Controller
    {
        //
        // GET: /StatSourceByHostPath/

        public string Index()
        {
            string strJson = "";
            string sPath = "";
            string startDate = "";
            string endDate = "";
            string callback = "";

            if (Request.QueryString["sPath"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null)
            {
                sPath = Request.QueryString["sPath"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();

                strJson = GetList(sPath, startDate, endDate);
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
        private string GetList(string sPath, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("HostPath", sPath),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59")
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_sourceByHostPath", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

    }
}
