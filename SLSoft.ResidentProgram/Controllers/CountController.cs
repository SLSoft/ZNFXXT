using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Caching;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;

namespace SLSoft.ResidentProgram.Controllers
{
    public class CountController : Controller
    {
        //
        // GET: /Count/
        DBFactory df = new DBFactory();
        AbstractDB db = null;
        DataTable dtSession = null;
        DataTable dtAccessList = null;
        int count = 0;

        public ActionResult Index()
        {
            dtSession = GetTableSession(1, "2013-08-02 00:00:00", "2013-08-31 00:00:00");
            dtAccessList = GetTableAccesslist(1, "2013-08-02 00:00:00", "2013-08-31 00:00:00"); 
            //ViewBag.PVcount =  GetPVCount();
            //ViewBag.UVcount = GetUVCount();
            //ViewBag.IPcount = GetIPCount();
            //ViewBag.AccessCoun = GetAccessCount();
            //ViewBag.PageCount = GetBrowsePageCount();
            //ViewBag.DepthCount = GetAvgDepthCount();
            //ViewBag.TimeSpent =GetDateDiff(GetTimeSpent());
            //ViewBag.AvgTimeSpent = GetDateDiff(GetAvgTimeSpent());

            ViewBag.icount = Insert_T_FlowAnalysis(1);

            return View();
        }

        #region 元数据加工
        /// <summary>
        /// 浏览次数PV 
        /// </summary>
        /// <returns></returns>
        private int GetPVCount()
        {
            if (dtAccessList != null)
            {
                count = dtAccessList.Rows.Count;
            }
            return count;
        }

        /// <summary>
        /// 独立访客数UV 访问网站的不重复用户数
        /// </summary>
        /// <param name="StatisticsSite_ID">站点ID</param>
        /// <returns></returns>

        private int GetUVCount()
        {
            if (dtSession != null)
            {
                count = dtSession.Rows.Count;
            }
            return count;
        }

        /// <summary>
        /// IP数 访问网站的不重复IP数
        /// </summary>
        /// <returns></returns>
        private int GetIPCount()
        {
            if (dtAccessList != null)
            {
                count = dtAccessList.DefaultView.ToTable(true, new string[] { "ClientIP"}).Rows.Count;
            }
            return count;
        }

        //新独立访客数
        private int GetNewUVCount()
        {
            return 0;
        }

        /// <summary>
        /// 访问次数:在一定时间范围内，网站所有访问者对网站访问的总次数，即访问者人数*每个访问者的访问次数
        /// </summary>
        /// <returns></returns>
        private int GetAccessCount()
        {
            if (dtSession != null)
            {
                string sum = dtSession.Compute("Sum(NumberOfVisits)", "NumberOfVisits > 0").ToString();
                if (string.IsNullOrEmpty(sum))
                {
                    return 0;
                }
                else
                {
                    count = int.Parse(sum);
                }
            }
            return count;
        }

        /// <summary>
        /// 人均浏览页数=浏览次数/独立访客
        /// </summary>
        /// <returns></returns>
        private string GetBrowsePageCount()
        {
            double mean_count = 0;
            if (GetUVCount() != 0)
            {
                mean_count = double.Parse(GetPVCount().ToString()) / GetUVCount();
            }
            return mean_count.ToString("f2");
        }

        /// <summary>
        /// 平均访问深度=浏览次数/访问次数
        /// </summary>
        /// <returns></returns>
        private string GetAvgDepthCount()
        {
            double depth_count = 0;
            if (GetAccessCount() != 0)
            {
                depth_count = double.Parse(GetPVCount().ToString()) / GetAccessCount();
            }
            return depth_count.ToString("f2");
        }

        /// <summary>
        /// 访问时长
        /// </summary>
        /// <returns></returns>
        private int GetTimeSpent()
        {
            if (dtSession != null)
            {
                string sum = dtSession.Compute("Sum(LengthOfSession)", "LengthOfSession > 0").ToString();
                if (string.IsNullOrEmpty(sum))
                {
                    return 0;
                }
                else
                {
                    count = int.Parse(sum);
                }
            }
            return count;
        }

        /// <summary>
        /// 平均访问时长
        /// </summary>
        /// <returns></returns>
        private int GetAvgTimeSpent()
        {
            if (GetAccessCount() != 0)
            {
                count = GetTimeSpent() / GetAccessCount();
            }
            return count;
        }
        private string GetDateDiff(int time)
        {
            string dateDiff = "";
            TimeSpan ts = new TimeSpan(0,0,time);
            if (ts.Days != 0)
            {
                dateDiff = ts.Days.ToString() + "天";
            }
            if(ts.Hours != 0)
            {
                dateDiff += ts.Hours.ToString() + "小时" ;
            }
            if (ts.Minutes != 0)
            {
                dateDiff += ts.Minutes.ToString() + "分钟";
            }
            if (ts.Seconds != 0)
            {
                dateDiff += ts.Seconds.ToString() + "秒";
            }
            return dateDiff;
        }

        /// <summary>
        /// 跳出次数
        /// </summary>
        /// <returns></returns>
        private int GetOutCount()
        {
            if (dtSession != null)
            {
                string sum = dtSession.Compute("Sum(NumberOfVisits)", "NumberOfVisits = 1").ToString();
                if (string.IsNullOrEmpty(sum))
                {
                    return 0;
                }
                else
                {
                    count = int.Parse(sum);
                }
            }
            return count;
        }
        /// <summary>
        /// 跳出率=跳出次数/访问次数
        /// </summary>
        /// <returns></returns>
        private int GetAvgOutCount()
        {
            if (GetAccessCount() != 0)
            {
                count = GetOutCount() / GetAccessCount();
            }
            return count;
        }
        #endregion

        #region 缓存

        //缓存访问表
        private DataTable GetTableSession(int Site_ID, string Start_Time, string End_Time)
        {
            Cache _cache = HttpRuntime.Cache;
            DataTable dt = null;
            db = df.CreateDB("mysql");

            if (_cache["slsoft_t_session_"+Site_ID] == null)
            {
                dt = db.ExecSql(string.Format("select * from slsoft_ias_bus_t_session where StatisticsSite_ID={0} and CreateTime between '{1}' and '{2}'",Site_ID,Start_Time,End_Time));
                _cache.Insert("slsoft_t_session_"+Site_ID, dt, null, DateTime.Now.AddMinutes(60 * 24), TimeSpan.Zero);
            }
            else
            {
                dt = (DataTable)_cache["slsoft_t_session_"+Site_ID];
            }
            return dt;
        }

        //缓存访问明细表
        private DataTable GetTableAccesslist(int Site_ID,string Start_Time,string End_Time)
        {
            Cache _cache = HttpRuntime.Cache;
            DataTable dt = null;
            db = df.CreateDB("mysql");

            if (_cache["slsoft_t_accesslist_"+Site_ID] == null)
            {
                dt = db.ExecSql(string.Format("select * from slsoft_ias_bus_t_accesslist where StatisticsSite_ID={0} and CreateTime between '{1}' and '{2}'", Site_ID, Start_Time, End_Time));
                _cache.Insert("slsoft_t_accesslist_"+Site_ID, dt, null, DateTime.Now.AddMinutes(60 * 24), TimeSpan.Zero);
            }
            else
            {
                dt = (DataTable)_cache["slsoft_t_accesslist_"+Site_ID];
            }
            return dt;
        }
        #endregion

        #region 添加信息到流量分析表
        private int Insert_T_FlowAnalysis(int SId)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("StatisticsSite_ID", SId),
                new MySqlParameter("StatisticsSite_Code", Guid.NewGuid().ToString()),
                new MySqlParameter("TimeZone", 8),
                new MySqlParameter("PV", GetPVCount()),
                new MySqlParameter("UV", GetUVCount()),
                new MySqlParameter("IP", GetIPCount()),
                new MySqlParameter("NewUV", GetNewUVCount()),
                new MySqlParameter("Session", GetAccessCount()),
                new MySqlParameter("ThePerCapitaBrowsingPages",GetBrowsePageCount()),
                new MySqlParameter("Name", GetAvgDepthCount()),
                new MySqlParameter("WorkUnit", GetAvgTimeSpent()),
                new MySqlParameter("MobilePhone", GetAvgOutCount())
            };
            return db.ExecProNoquery("slsoft_ias_bus_p_insert_flowanalysis", mpara);
        }
        #endregion
    }
}
