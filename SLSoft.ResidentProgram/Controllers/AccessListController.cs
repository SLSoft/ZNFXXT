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
        // 根据会话code查看访问明细
        // GET: /AccessList/

        public string Index(string sCode,string sDate)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sCode) && !string.IsNullOrEmpty(sDate))
            {
                strJson = GetAccessList(sCode,sDate);
            }
            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 根据访问编码获得访问明细
        /// </summary>
        /// <param name="SessionCode"></param>
        /// <returns></returns>
        private string GetAccessList(string sCode,string sDate)
        {
            string strJson = "";
            DataTable dt = null;

            if (sDate == DateTime.Now.Date.ToString("yyyy-MM-dd"))
            {
                dt = GetDayList(sCode);
            }
            else
            {
                dt = GetHisList(sCode);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            return strJson;
        }

        #region 获取明细列表数据

        DBFactory df = new DBFactory();
        AbstractDB db = null;

        /// <summary>
        /// 获取当天会话明细列表
        /// </summary>
        /// <param name="SessionCode"></param>
        /// <returns></returns>
        private DataTable GetDayList(string SessionCode)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SessionCode", SessionCode)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_day_AccessDetail", mpara);
            return dt;
        }

        /// <summary>
        /// 获取历史会话明细列表
        /// </summary>
        /// <param name="SessionCode"></param>
        /// <returns></returns>
        private DataTable GetHisList(string SessionCode)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SessionCode", SessionCode)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_select_His_AccessDetail", mpara);
            return dt;
        }
        #endregion
    }
}
