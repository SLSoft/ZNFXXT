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
    public class AccessListController : Controller
    {
        //
        // GET: /AccessList/

        public string Index()
        {
            string strJson = "";
            string sessionCode = "";
            string callback = "";

            if (Request.QueryString["sCode"] != null)
            {
                sessionCode = Request.QueryString["sCode"].ToString();

                strJson = GetAccessList(sessionCode);
            }
            callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 根据访问编码获得访问明细
        /// </summary>
        /// <param name="SessionCode"></param>
        /// <returns></returns>
        private string GetAccessList(string SessionCode)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SessionCode", SessionCode)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_accesslistBySessionCode", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        } 
    }
}
