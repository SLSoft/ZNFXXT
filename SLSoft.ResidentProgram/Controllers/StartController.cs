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
using System.Text;

namespace SLSoft.ResidentProgram.Controllers
{
    public class StartController : BaseController
    {
        //
        // GET: /Start/

        DBFactory df = new DBFactory();
        AbstractDB db = null;
        DataTable dtIP = null;
        DataRow[] drIP = null;
        /* 测试用参数
        string sId = "1";
        string showId = "1";*/

        /* 发布用参数*/
        public string sId = "0";
        string showId = "0";
        
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

                    if (Request.Cookies["siteID"] != null)
                    {
                        Request.Cookies.Remove("siteID");
                    }

                    strCode = getCode(sId, showId);//获取符合要求的js
                }
            }
            catch (Exception)
            {
            }
            return strCode;
        }

        /// <summary>
        /// 获取符合要求的js驻留代码
        /// </summary>
        /// <param name="sId">网站id</param>
        /// <param name="show_id">效果id</param>
        /// <returns></returns>
        public string getCode(string s_Id, string show_id)
        {
            string strReturn ="";

            strReturn = System.IO.File.ReadAllText(Server.MapPath("Scripts/gd.js"));

            return strReturn;
        }
        
        public ActionResult GetData(FormCollection fc)
        {
            if (fc.Count == 0)
            {
                fc = SetPostData(GetDocumentContents(Request));
            }
            Information im = FormatMessage(fc);

            dtIP = GetTableIP();
            drIP = GetIPInfo(im.UserHostAddress);
            Session["drIP"] = drIP;

            SaveInformation(im);

            return Json(new{ });
        }

        //将参数设置为键值格式
        public FormCollection SetPostData(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return null;

            string[] arry = strData.Split('&');
            FormCollection fc = new FormCollection();

            for (int i = 0; i < arry.Length; i++)
            {
                string[] items = arry[i].Split('=');
                fc.Add(items[0], Server.UrlDecode(items[1]));
            }
            return fc;
        }
        public ActionResult setStr()
        {
            return View();
        }
        private string GetDocumentContents(System.Web.HttpRequestBase Request)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

        #region 获取客户端信息
        private Information FormatMessage(FormCollection fc)
        {
            Information im = new Information();

            //js数据
            im.sId = fc["sId"];//站点id
            im.IsMove = fc["isMove"]; //是否移动终端
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
            im.CurrentName = fc["cName"];//当前域名
            im.CurrentUrl = fc["cUrl"];//当前URL
            im.currentUrlTitle = fc["cUrlTitle"];//当前URL标题
            im.ParentUrl = fc["parentUrl"];//父窗口URL
            im.ClientX = fc["clientX"];
            im.ClientY = fc["clientY"];
            im.UserHostAddress = fc["IP"]==""? Request.UserHostAddress:fc["IP"].ToString().Trim();//客户端IP

            //cookie数据
            im.UserCode = fc["userCode"].ToString();
            im.SessionCode = fc["session"].ToString();
            im.SessionType = fc["stype"].ToString();
            im.ApplicationType = fc["atype"].ToString();
            im.OperType = fc["otype"].ToString();
            im.AccessCount = fc["count"].ToString();
            im.LastAccessTime = DateTime.Now.ToString();

            //服务器数据
            im.UserHostName = System.Net.Dns.GetHostName();//客户端计算机名
            im.ServerAddress = Request.ServerVariables["LOCAL_ADDR"];//服务器端IP
            im.JavaApplets = Request.Browser.JavaApplets;//是否支持JAVA
            im.Frames = Request.Browser.Frames;//是否支持框架网页

            return im;
        }
        #endregion

        #region 操作数据
        private void SaveInformation(Information im)
        {
            try
            {
                if (im.OperType == "0")
                {
                    Insert_T_Session(im);
                }
                else
                {
                    Update_T_Session(im);
                    Update_T_SessionDetails(im);//修改上一条访问时长
                }
                Insert_T_SessionDetails(im);
            }
            catch (Exception)
            {
            }
        }

        //添加信息到访问表
        private void Insert_T_Session(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("Code",im.SessionCode==null?Guid.NewGuid().ToString():im.SessionCode),
                new MySqlParameter("UserCode",im.UserCode),
                new MySqlParameter("StatisticsSite_ID", im.sId),
                new MySqlParameter("StatisticsSite_Code", Guid.NewGuid().ToString()),
                new MySqlParameter("SourceClass_ID", GetSourceClassID(im.PageUpUrl,im.CurrentName)),
                new MySqlParameter("SourceClass_Code", GetSoureName(im.PageUpUrl).Split('|').GetValue(0).ToString()),
                new MySqlParameter("SourcePath", im.PageUpUrl),
                new MySqlParameter("SourceKey",GetSoureName(im.PageUpUrl).Split('|').GetValue(1).ToString()),
                new MySqlParameter("LastAccessPage", im.CurrentUrl),
                new MySqlParameter("LengthOfSession", "0"),//访问时长
                new MySqlParameter("SessionDepth", 1),
                new MySqlParameter("TimeZone", im.Zone),
                new MySqlParameter("IsUV", IsUV(im.UserHostAddress,im.SessionType,Request.UserAgent,im.IsCookie)),
                new MySqlParameter("IsNewUV", IsNewUV(im.UserHostAddress,im.ApplicationType,Request.UserAgent,im.IsCookie)),
                new MySqlParameter("LastBrowsingTime", im.LastAccessTime),//上次访问时间
                new MySqlParameter("NumberOfVisits", im.AccessCount),
                new MySqlParameter("NetworkAccessProvider", GetNAPByIPAddress()),
                new MySqlParameter("Language", im.SysLanguage),
                new MySqlParameter("DeviceType", im.IsMove=="否"?"0":"1"),
                new MySqlParameter("AboutDevice", im.MoveType),
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

        //添加信息到访问明细表
        private void Insert_T_SessionDetails(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara = {
                new MySqlParameter("StatisticsSite_ID", im.sId),
                new MySqlParameter("StatisticsSite_Code", Guid.NewGuid().ToString()),
                new MySqlParameter("Session_Code", im.SessionCode),
                new MySqlParameter("SourceClass_ID", GetSourceClassID(im.PageUpUrl,im.CurrentName)),
                new MySqlParameter("SourceClass_Code", GetSoureName(im.PageUpUrl).Split('|').GetValue(0).ToString()),
                new MySqlParameter("SourcePath", im.PageUpUrl),
                new MySqlParameter("SourceKey",GetSoureName(im.PageUpUrl).Split('|').GetValue(1).ToString()),
                new MySqlParameter("LastAccessPage", im.CurrentUrl),
                new MySqlParameter("LengthOfPage", "0"),//停留时长
                new MySqlParameter("SessionDepth", 1),
                new MySqlParameter("TimeZone", im.Zone),
                new MySqlParameter("IsUV", IsUV(im.UserHostAddress,im.SessionType,Request.UserAgent,im.IsCookie)),
                new MySqlParameter("IsNewUV", IsNewUV(im.UserHostAddress,im.ApplicationType,Request.UserAgent,im.IsCookie)),
                new MySqlParameter("LastBrowsingTime", im.LastAccessTime),
                new MySqlParameter("NumberOfVisits", 1),
                new MySqlParameter("NetworkAccessProvider", GetNAPByIPAddress()),
                new MySqlParameter("Language", im.SysLanguage),
                new MySqlParameter("DeviceType", im.IsMove=="否"?"0":"1"),
                new MySqlParameter("AboutDevice", im.MoveType),
                new MySqlParameter("OperationSystem", im.OS),
                new MySqlParameter("ClientX",im.ClientX),
                new MySqlParameter("ClientY",im.ClientY),
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

        //修改访问表
        private void Update_T_Session(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara = {
                new MySqlParameter("lastAccessPage", im.CurrentUrl),
                new MySqlParameter("SessionCode", im.SessionCode)
            };
            db.ExecProNoquery("slsoft_ias_bus_p_update_session", mpara);
        }

        //修改访问明细表（修改访问时长）
        private void Update_T_SessionDetails(Information im)
        {
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara = new MySqlParameter[2];
            mpara[0] = new MySqlParameter("SessionCode", im.SessionCode);
            mpara[1] = new MySqlParameter("LastTime","");
            mpara[1].MySqlDbType = MySqlDbType.VarChar;
            mpara[1].Direction = ParameterDirection.Output;

            db.ExecProcedure("slsoft_ias_bus_p_update_accesslist", mpara);
            im.LastAccessTime = mpara[1].Value.ToString()==""?DateTime.Now.ToString():mpara[1].Value.ToString();
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
        private int IsUV(string ClientIP,string SessionType,string UA,string IsCookie)
        {
            if (IsCookie == "1")
            {
                if (SessionType == "1")
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
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("IP",ClientIP),
                new MySqlParameter("UAMD5", FormsAuthentication.HashPasswordForStoringInConfigFile(UA,"md5"))
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
        /// <returns></returns>
        private int IsNewUV(string ClientIP, string ApplicationType, string UA, string IsCookie)
        {
            if (IsCookie == "1")
            {
                if (ApplicationType == "1")
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
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("IP",ClientIP),
                new MySqlParameter("UAMD5", FormsAuthentication.HashPasswordForStoringInConfigFile(UA,"md5"))
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
                DataRow[] row = dtIP.Select(string.Format("'{0}'>= AIP and '{0}' <= BIP", ip));
                if (row.Count() > 0)
                {
                    return row;
                }
            }
            return null;
        }

        Common.SearchKeyword searchkey = new Common.SearchKeyword();

        //访问来源类别（1.直接输入网址或书签 2.搜索引擎 3.内部链接 4.外部链接）
        private int GetSourceClassID(string sourceurl,string currurl)
        {
            if (string.IsNullOrEmpty(sourceurl))
                return 1;
            if (searchkey.IsSearchEnginesGet(sourceurl))
                return 2;
            if (sourceurl.IndexOf(currurl) != -1)
                return 3;
            return 4;
        }

        //分析搜索引擎url 得到引擎名称
        private string GetSoureName(string sourceurl)
        {
            string Keyword = "";
            string Engine = "";
            if (!string.IsNullOrEmpty(sourceurl))
            {
                //判断是否搜索引擎链接
                if (searchkey.IsSearchEnginesGet(sourceurl))
                {
                    //取得搜索关键字
                    Keyword = searchkey.SearchKey(sourceurl);
                    //取得搜索引擎名称
                    Engine = searchkey.EngineName;
                }
            }
            return Engine + "|" + Keyword;
        }
        #endregion
    }
}
