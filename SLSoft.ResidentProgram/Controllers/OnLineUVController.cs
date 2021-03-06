﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Data;

namespace SLSoft.ResidentProgram.Controllers
{
    public class OnLineUVController : Controller
    {
        // 当前在线（最近XX分钟新、老访客）
        // GET: /OnLineUV/

        public string Index(string sId, string iTime)
        {
            string strJson = "";

            if (!string.IsNullOrEmpty(sId) && !string.IsNullOrEmpty(iTime))
            {
                strJson = GetInfo(int.Parse(sId), int.Parse(iTime));
            }
            string callback = HttpContext.Request["callback"];
            return callback + "(" + strJson + ")";
        }

        /// <summary>
        /// 获取当前在线（最近XX分钟新、老访客）
        /// </summary>
        /// <param name="SiteID"></param>
        /// <param name="IntervalTime"></param>
        /// <returns></returns>
        private string GetInfo(int SiteID, int IntervalTime)
        {
            string strJson = "";
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", SiteID),
                new MySqlParameter("IntervalTime", IntervalTime)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_day_OnLineUV", mpara);

            if (dt != null && dt.Rows.Count > 0)
            {
                strJson = Common.JsonHelper.ToJson(dt);
            }
            else
            {
                strJson = "[{NewUV:0,UV:0}]";
            }
            return strJson;
        } 
    }
}