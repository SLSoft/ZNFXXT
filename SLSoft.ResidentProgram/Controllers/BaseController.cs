using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Caching;
using SLSoft.EasyORM;
using System.Net;

namespace SLSoft.ResidentProgram.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        #region 设置cookie
        public void setCookie(string sId)
        {
            if (Request.Cookies[string.Format("SLSoft_IA_{0}", sId)] == null)
            {
                CreateCookie(sId);
            }
            else
            {
                UpdateCookie(sId);
            }
        }

        public void CreateCookie(string sId)
        {
            HttpCookie MyCookie = new HttpCookie(string.Format("SLSoft_IA_{0}", sId));

            MyCookie["SessionCode"] = Guid.NewGuid().ToString();
            MyCookie["SessionType"] = "0";//0:代表新session 1:代表有效session
            MyCookie["ApplicationCode"] = Guid.NewGuid().ToString();
            MyCookie["ApplicationType"] = "0";//0：代表新Application
            MyCookie["LastAccessTime"] = DateTime.Now.ToString();//最后一次访问时间
            MyCookie["NowTime"] = DateTime.Now.ToString();
            MyCookie.Expires = DateTime.Now.AddDays(7);
            System.Web.HttpContext.Current.Response.Cookies.Add(MyCookie);
            Session[string.Format("SLSoft_IA_{0}", sId)] = MyCookie["SessionCode"].ToString();
        }

        public void UpdateCookie(string sId)
        {
            HttpCookie MyCookie = Request.Cookies[string.Format("SLSoft_IA_{0}", sId)];

            if (Session[string.Format("SLSoft_IA_{0}", sId)] == null)
            {
                Session[string.Format("SLSoft_IA_{0}", sId)] = Guid.NewGuid().ToString();
                MyCookie["SessionType"] = "0";//0:代表新session 1:代表有效session
                MyCookie["SessionCode"] = Session[string.Format("SLSoft_IA_{0}", sId)].ToString();
            }
            else
            {
                MyCookie["SessionType"] = "1";//0:代表新session 1:代表有效session
            }
            MyCookie["ApplicationType"] = "1";//0:代表新Application 1:代表旧的Application
            MyCookie["LastAccessTime"] = DateTime.Now.ToString();

            System.Web.HttpContext.Current.Response.Cookies.Set(MyCookie);
        }
        #endregion

        #region 缓存IP表
        public DataTable GetTableIP()
        {
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            DataTable dt = null;
            db = df.CreateDB("mysql");
            
            object locker = new object();
            Cache _cache = HttpRuntime.Cache;

            if (_cache["slsoft_t_IP"] == null)
            {
                lock (locker)
                {
                    dt = db.ExecSql("select * from slsoft_ias_bus_t_ip");
                    _cache.Insert("slsoft_t_IP", dt, null, new DateTime(2099, 12, 31), TimeSpan.Zero);
                }
            }
            else
            {
                dt = (DataTable)_cache["slsoft_t_IP"];
            }
            return dt;
        }
        #endregion
    }
}
