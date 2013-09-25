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
    public class StatAccessByHostController : Controller
    {
        //
        // GET: /StatAccessByHost/

        public string Index()
        {
            string strJson = "";
            string sId = "";
            string sPath = "";
            string startDate = "";
            string endDate = "";
            string callback = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["sPath"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null)
            {
                sId = Request.QueryString["sId"].ToString();
                sPath = Request.QueryString["sPath"].ToString();
                startDate = Request.QueryString["startDate"].ToString();
                endDate = Request.QueryString["endDate"].ToString();

                strJson = GetList(sId,sPath, startDate, endDate);
            }
            callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 根据受访域名查看明细
        /// </summary>
        /// <param name="sId"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string GetList(string sId,string sPath, string startDate, string endDate)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate+" 00:00:00"),
                new MySqlParameter("endDate",endDate+" 23:59:59"),
                new MySqlParameter("HostPath",sPath)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_AccessByHostPath", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }
    }
}
