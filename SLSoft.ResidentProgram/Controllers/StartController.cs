using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SLSoft.ResidentProgram.Models;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;
using System.Web.Caching;
using System.Web.Security;
using System.Data;

namespace SLSoft.ResidentProgram.Controllers
{
    public class StartController : Controller
    {
        //
        // GET: /Start/

        DBFactory df = new DBFactory();
        AbstractDB db = null;
        DataTable dtIP = null;
        DataRow[] drIP = null;
        /* 测试用参数*/
        string sId = "1";
        string showId = "1";

        /* 发布用参数
        string sId = "0";
        string showId = "0";
        */

        /// <summary>
        /// 根据参数返回js驻留脚本
        /// </summary>
        /// <returns></returns>
        public string Index()
        {
            string strCode="";
            try
            {
                if (Request.QueryString["sId"] != null && Request.QueryString["show_id"] != null)
                {
                    sId = Request.QueryString["sId"].ToString();
                    showId = Request.QueryString["show_id"].ToString();

                    strCode = getCode(sId, showId);//获取符合要求的js

                    setCookie(sId);//设置cookie值

                    dtIP = GetTableIP();
                    drIP = GetIPInfo(Request.UserHostAddress);
                    Session["drIP"] = drIP;

                }
            }
            catch (Exception)
            {
            }
            //ViewBag.strCode = strCode;
            return strCode;
        }

        #region 设置cookie
        private void setCookie(string sId)
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

        private void CreateCookie(string sId)
        {
            HttpCookie MyCookie = new HttpCookie(string.Format("SLSoft_IA_{0}", sId));

            MyCookie["SessionCode"] = Guid.NewGuid().ToString();
            MyCookie["SessionType"] = "0";//0:代表新session 1:代表有效session
            MyCookie["ApplicationCode"] = Guid.NewGuid().ToString();
            MyCookie["ApplicationType"] = "0";//0：代表新Application
            MyCookie["LastAccessTime"] = DateTime.Now.ToString();//最后一次访问时间
            MyCookie["NowTime"] = DateTime.Now.ToString();
            MyCookie.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Add(MyCookie);
            Session[string.Format("SLSoft_IA_{0}", sId)] = MyCookie["SessionCode"].ToString();
        }

        private void UpdateCookie(string sId)
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
            Session["AccessLength"] = GetLengthOfSession();
            MyCookie["LastAccessTime"] = DateTime.Now.ToString();

            Response.Cookies.Set(MyCookie);
        }
        #endregion

        /// <summary>
        /// 获取符合要求的js驻留代码
        /// </summary>
        /// <param name="sId">网站id</param>
        /// <param name="show_id">效果id</param>
        /// <returns></returns>
        private string getCode(string sId, string show_id)
        {
            string strReturn ="";

            strReturn = System.IO.File.ReadAllText(Server.MapPath("Scripts/gd.js"));

            return strReturn;
        }

        public ActionResult Context()
        {
            return View();
        }

        public ActionResult GetData(FormCollection fc)
        {
            Information im = FormatMessage(fc);

            SaveInformation(im);

            return View();
        }

        #region 获取客户端信息
        private Information FormatMessage(FormCollection fc)
        {
            Information im = new Information();

            //js数据
            im.IsMove = fc["isMove"];//是否移动终端
            im.MoveType = fc["moveType"];//移动终端类型
            im.BrowserType = fc["browserType"];//浏览器类型
            im.BrowserKernel = fc["browserKernel"];//浏览器内核
            im.BVersions = fc["bVersions"];//浏览器版本
            im.BLanguage = fc["bLanguage"];//浏览器语言
            im.SysLanguage = fc["sysLanguage"];//系统语言
            im.UserLanguage = fc["userLanguage"];//用户语言
            im.CpuType = fc["cpuType"];//CPU类型
            im.OS = fc["OS"];//操作系统
            im.Size = fc["size"];//分辨率
            im.IsCookie = fc["isCookie"];//是否支持cookie
            im.Plugins = fc["plugins"];//插件
            im.VColor = fc["vColor"];//色彩
            im.Zone = fc["zone"];//时区
            im.PageUpUrl = fc["pageUpUrl"];//上一页URL
            im.CurrentName = fc["currentName"];//当前域名
            im.CurrentUrl = fc["currentUrl"];//当前URL
            im.currentUrlTitle = fc["currentUrlTitle"];//当前URL标题
            im.ParentUrl = fc["parentUrl"];//父窗口URL

            //cookie数据
            HttpCookie cookie = Request.Cookies[string.Format("SLSoft_IA_{0}", sId)];
            im.SessionCode = cookie["SessionCode"].ToString();
            im.SessionType = cookie["SessionType"].ToString();
            im.ApplicationCode = cookie["ApplicationCode"].ToString();
            im.ApplicationType = cookie["ApplicationType"].ToString();
            im.LastAccessTime = cookie["LastAccessTime"].ToString();

            //服务器数据
            im.UserHostName = System.Net.Dns.GetHostName();//客户端计算机名
            im.UserHostAddress = Request.UserHostAddress;//客户端IP
            im.ServerAddress = Request.ServerVariables["LOCAL_ADDR"];//服务器端IP
            im.JavaApplets = Request.Browser.JavaApplets;//是否支持JAVA
            im.Frames = Request.Browser.Frames;//是否支持框架网页

            return im;
        }
        #endregion

        #region 操作数据
        private void SaveInformation(Information im)
        {
            if (im.SessionType == "0")
            {
                Insert_T_Session(im);
            }
            else
            {
                Update_T_Session(im);
            }
            Insert_T_SessionDetails(im);

            Cache _cache = HttpRuntime.Cache;
            _cache.Remove("slsoft_t_session_" + sId);
            _cache.Remove("slsoft_t_accesslist_" + sId);
        }

        //添加信息到访问表
        private void Insert_T_Session(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("Code",im.SessionCode),
                new MySqlParameter("StatisticsSite_ID", 1),
                new MySqlParameter("StatisticsSite_Code", Guid.NewGuid().ToString()),
                new MySqlParameter("SourceClass_ID", GetSourceClassID(im.PageUpUrl)),
                new MySqlParameter("SourceClass_Code", 1),
                new MySqlParameter("SourcePath", im.PageUpUrl),
                new MySqlParameter("LastAccessPage", im.CurrentName),
                new MySqlParameter("LengthOfSession", "0"),
                new MySqlParameter("SessionDepth", 1),
                new MySqlParameter("TimeZone", im.Zone),
                new MySqlParameter("IsUV", IsUV(Request.UserHostAddress,Request.Cookies[string.Format("SLSoft_IA_{0}", sId)]["SessionType"],Request.UserAgent,im.IsCookie)),
                new MySqlParameter("IsNewUV", IsNewUV(Request.UserHostAddress,Request.Cookies[string.Format("SLSoft_IA_{0}", sId)]["ApplicationType"],Request.UserAgent,im.IsCookie)),
                new MySqlParameter("LastBrowsingTime", DateTime.Now),
                new MySqlParameter("NumberOfVisits", 1),
                new MySqlParameter("NetworkAccessProvider", GetNAPByIPAddress()),
                new MySqlParameter("Language", im.SysLanguage),
                new MySqlParameter("DeviceType", "0"),
                new MySqlParameter("AboutDevice", ""),
                new MySqlParameter("OperationSystem", im.OS),
                new MySqlParameter("Resolution", im.Size),
                new MySqlParameter("Color", im.VColor),
                new MySqlParameter("UserAgent", Request.UserAgent),
                new MySqlParameter("UserAgentMD5", FormsAuthentication.HashPasswordForStoringInConfigFile(Request.UserAgent, "md5")),
                new MySqlParameter("Browser", im.BrowserType),
                new MySqlParameter("BrowserEdition", im.BVersions),
                new MySqlParameter("BrowserKernel", im.BrowserKernel),
                new MySqlParameter("IsCookieSupport", im.IsCookie),
                new MySqlParameter("IsJavaSupport", im.JavaApplets),
                new MySqlParameter("IsFrameWebpageSupport", im.Frames),
                new MySqlParameter("Plugin", ""),
                new MySqlParameter("ClientIP", im.UserHostAddress),
                new MySqlParameter("Country", "中国"),
                new MySqlParameter("Province", GetProvinceByIP()),
                new MySqlParameter("City", GetCityByIP()),
                new MySqlParameter("Area", GetAreaByIP()),
                new MySqlParameter("ServerIP", im.ServerAddress)
            };
            db.ExecProNoquery("slsoft_ias_bus_p_insert_session", mpara);
        }

        //修改访问表
        private void Update_T_Session(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara = {
                new MySqlParameter("lastAccessPage", im.CurrentUrl),
                new MySqlParameter("LengthOfSessionStep",Session["AccessLength"].ToString()),
                new MySqlParameter("SessionCode", im.SessionCode)
            };
            db.ExecProNoquery("slsoft_ias_bus_p_update_session", mpara);
        }

        //添加信息到访问明细表
        private void Insert_T_SessionDetails(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara = {
                new MySqlParameter("StatisticsSite_ID", 1),
                new MySqlParameter("StatisticsSite_Code", Guid.NewGuid().ToString()),
                new MySqlParameter("Session_Code", im.SessionCode),
                new MySqlParameter("SourceClass_ID", GetSourceClassID(im.PageUpUrl)),
                new MySqlParameter("SourceClass_Code", 1),
                new MySqlParameter("SourcePath", im.PageUpUrl),
                new MySqlParameter("LastAccessPage", im.CurrentName),
                new MySqlParameter("LengthOfPage", "0"),
                new MySqlParameter("SessionDepth", 1),
                new MySqlParameter("TimeZone", im.Zone),
                new MySqlParameter("IsUV", IsUV(Request.UserHostAddress,Request.Cookies[string.Format("SLSoft_IA_{0}", sId)]["SessionType"],Request.UserAgent,im.IsCookie)),
                new MySqlParameter("IsNewUV", IsNewUV(Request.UserHostAddress,Request.Cookies[string.Format("SLSoft_IA_{0}", sId)]["ApplicationType"],Request.UserAgent,im.IsCookie)),
                new MySqlParameter("LastBrowsingTime", DateTime.Now),
                new MySqlParameter("NumberOfVisits", 1),
                new MySqlParameter("NetworkAccessProvider", GetNAPByIPAddress()),
                new MySqlParameter("Language", im.SysLanguage),
                new MySqlParameter("DeviceType", "0"),
                new MySqlParameter("AboutDevice", ""),
                new MySqlParameter("OperationSystem", im.OS),
                new MySqlParameter("Resolution", im.Size),
                new MySqlParameter("Color", im.VColor),
                new MySqlParameter("UserAgent", Request.UserAgent),
                new MySqlParameter("UserAgentMD5", FormsAuthentication.HashPasswordForStoringInConfigFile(Request.UserAgent, "md5")),
                new MySqlParameter("Browser", im.BrowserType),
                new MySqlParameter("BrowserEdition", im.BVersions),
                new MySqlParameter("BrowserKernel", im.BrowserKernel),
                new MySqlParameter("IsCookieSupport", im.IsCookie),
                new MySqlParameter("IsJavaSupport", im.JavaApplets),
                new MySqlParameter("IsFrameWebpageSupport", im.Frames),
                new MySqlParameter("Plugin", ""),
                new MySqlParameter("ClientIP", im.UserHostAddress),
                new MySqlParameter("Country", "中国"),
                new MySqlParameter("Province", GetProvinceByIP()),
                new MySqlParameter("City", GetCityByIP()),
                new MySqlParameter("Area", GetAreaByIP()),
                new MySqlParameter("ServerIP", im.ServerAddress)
            };
            db.ExecProNoquery("slsoft_ias_bus_p_insert_accesslist", mpara);
        }
        #endregion

        #region 获取参数方法
        /// <summary>
        /// 是否独立访客（0:不是;1是）
        /// </summary>
        /// <param name="ClientIP"></param>
        /// <param name="CookieType"></param>
        /// <param name="UA"></param>
        /// <returns></returns>
        private int IsUV(string ClientIP,string CookieType,string UA,string IsCookie)
        {
            if (IsCookie == "1")
            {
                if (CookieType == "0")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                db = df.CreateDB("mysql");

                MySqlParameter[] mpara = {
                new MySqlParameter("StatisticsSite_ID", sId),
                new MySqlParameter("ClientIP",ClientIP),
                new MySqlParameter("UserAgentMD5", FormsAuthentication.HashPasswordForStoringInConfigFile(UA,"md5"))
                };
                string value = db.ExecProcedure("slsoft_ias_bus_p_check_isUV", mpara).Rows[0][0].ToString();
                if (value == "0")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 是否新的独立访客（0:不是;1是）
        /// </summary>
        /// <param name="ClientIP"></param>
        /// <param name="CookieMessage"></param>
        /// <param name="UA"></param>
        /// <param name="IsCookie"></param>
        /// <returns></returns>
        private int IsNewUV(string ClientIP, string CookieType, string UA, string IsCookie)
        {
            if (IsCookie == "1")
            {
                if (CookieType == "0")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                db = df.CreateDB("mysql");

                MySqlParameter[] mpara = {
                new MySqlParameter("StatisticsSite_ID", sId),
                new MySqlParameter("ClientIP",ClientIP),
                new MySqlParameter("UserAgentMD5", FormsAuthentication.HashPasswordForStoringInConfigFile(UA,"md5"))
                };
                string value = db.ExecProcedure("slsoft_ias_bus_p_check_isNewUV", mpara).Rows[0][0].ToString();
                if (value == "0")
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        //获取IP归属地 省市区
        private string GetProvinceByIP()
        {
            if (Session["drIP"] != null)
            {
                DataRow[] dr = (DataRow[])Session["drIP"];
                return dr[0]["Province"].ToString();
            }
            else
            {
                return "未知";
            }
        }
        private string GetCityByIP()
        {
            if (Session["drIP"] != null)
            {
                DataRow[] dr = (DataRow[])Session["drIP"];
                return dr[0]["City"].ToString();
            }
            else
            {
                return "";
            }
        }
        private string GetAreaByIP()
        {
            if (Session["drIP"] != null)
            {
                DataRow[] dr = (DataRow[])Session["drIP"];
                return dr[0]["Area"].ToString();
            }
            else
            {
                return "";
            }
        }

        //获取IP所属网络接入商
        private string GetNAPByIPAddress()
        {
            if (Session["drIP"] != null)
            {
                DataRow[] dr = (DataRow[])Session["drIP"];
                return dr[0]["Service"].ToString();
            }
            else
            {
                return "未知";
            }
        }

        //IP转换为整形
        private string GetIpLong(string ClientIp)
        {
            string[] subIP = ClientIp.Split('.');

            long ip = 16777216 * Convert.ToInt64(subIP[0]) + 65536 * Convert.ToInt64(subIP[1]) + 256 * Convert.ToInt64(subIP[2]) + Convert.ToInt64(subIP[3]);

            return ip.ToString();
        }
        //获取ip段信息
        private DataRow[] GetIPInfo(string ClientIp)
        {
            string ip = GetIpLong(ClientIp);
            if (dtIP != null)
            {
                return dtIP.Select(string.Format("'{0}'>= AIP and '{0}' <= BIP", ip));
            }
            else
            {
                return null;
            }
        }

        //访问来源类别（1.直接输入网址或书签 2.搜索引擎 3.内部链接 4.外部链接）
        private int GetSourceClassID(string sourceurl)
        {
            if (sourceurl == "")
                return 1;
            if (sourceurl.IndexOf("baidu") > 0 || sourceurl.IndexOf("google") > 0)
                return 2;
            return 4;
        }

        //获取访问时长（秒）
        private int GetLengthOfSession()
        {
            DateTime NowTime = DateTime.Now;
            DateTime LastBrowsingTime = DateTime.Now;

            if (Request.Cookies["SLSoft_IA_1"]["LastAccessTime"] != null)
            {
                LastBrowsingTime = Convert.ToDateTime(Request.Cookies["SLSoft_IA_1"]["LastAccessTime"].ToString());
            }
            TimeSpan ts = NowTime - LastBrowsingTime;
            return ts.Seconds;
        }
        
        #endregion

        #region 缓存IP表
        private DataTable GetTableIP()
        {
            Cache _cache = HttpRuntime.Cache;
            DataTable dt = null;
            db = df.CreateDB("mysql");

            if (_cache["slsoft_t_IP"] == null)
            {
                dt = db.ExecSql("select * from slsoft_ias_bus_t_ip");
                _cache.Insert("slsoft_t_IP", dt, null, new DateTime(2099, 12, 31), TimeSpan.Zero);
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
