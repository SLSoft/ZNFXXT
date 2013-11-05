using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NSoup.Parse;
using NSoup.Nodes;
using NSoup.Helper;
using NSoup.Safety;
using NSoup.Select;
using System.Net;
using System.Text.RegularExpressions;
using SLSoft.EasyORM;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text;

namespace SLSoft.ResidentProgram.Controllers
{
    public class UserViewController : Controller
    {
        // 用户视点
        // GET: /UserView/

        public ActionResult Index(string sId, string strUrl, string sDate)
        {
            string host = "http://" + regexdom(strUrl);
            WebRequest webRequest = WebRequest.Create(strUrl);
            NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(webRequest.GetResponse().GetResponseStream(), "utf-8");

            Elements media = doc.Select("[src]");
            Elements links = doc.Select("a[href]");
            Elements imports = doc.Select("link[href]");
            Elements bg = doc.Select("[background]");
            doc.Body.Prepend("<div id='layoutdiv'></div>");
            foreach (Element eb in bg)
            {
                eb.Attr("background", host + setURL(eb.Attr("background")));
            }
            foreach (Element ei in imports)
            {
                if (ei.Attr("href").IndexOf("http") < 0)
                {
                    ei.Attr("href", host +setURL(ei.Attr("href")));
                }
            }
            foreach (Element em in media)
            {
                if (em.Attr("src").IndexOf("http") < 0)
                {
                    em.Attr("src", host + setURL(em.Attr("src")));
                }
            }

            int max = 0;
            int pvnum = 1;
            DataTable dtnum = GetList(sId, strUrl, sDate);
            DataTable dt = GetList2(sId, sDate);

            if (dtnum.Rows.Count > 0)
            {
                pvnum = int.Parse(dtnum.Rows[0]["num"].ToString());
            }
            foreach (Element el in links)
            {
                if (el.Attr("href").IndexOf("http") < 0)
                {
                    el.Attr("href", host + setURL(el.Attr("href")));
                }
                DataRow[] dr = dt.Select("path='" + el.Attr("href") + "'");

                if (dr.Count() > 0)
                {
                    if (int.Parse(dr[0]["pvnum"].ToString()) > max)
                    {
                        max = int.Parse(dr[0]["pvnum"].ToString());
                    }

                    el.Attr("pv", dr[0]["pvnum"].ToString());
                    el.Attr("uv", dr[0]["uvnum"].ToString());

                    float avgpv = (float.Parse(dr[0]["pvnum"].ToString()) / float.Parse(pvnum.ToString())) * 100;
                    el.Attr("pec", avgpv.ToString("0.000") + "%");
                }

                el.Attr("class", "linknode");
            }
            
            Element lodiv = doc.Select("#layoutdiv").First();
            lodiv.Attr("max", max.ToString());
            
            doc.Head.Append("<script src='../../Scripts/jquery-1.5.1.min.js' type='text/javascript'></script>");
            doc.Head.Append("<script src='../../Scripts/jquery.tipTip.minified.js' type='text/javascript'></script>");
            doc.Head.Append("<link href='../../Content/tipTip.css' rel='stylesheet' type='text/css' />");

            return Json(doc.OuterHtml(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Show()
        {
            return View();
        }

        private string regexdom(string url)
        {
            string text = url;
            string pattern = @"(?<=http://)[\w\.]+[^/]";　//C#正则表达式提取匹配URL的模式，       
            string s = "";
            MatchCollection mc = Regex.Matches(text, pattern);//满足pattern的匹配集合        
            foreach (Match match in mc)
            {
                s = match.ToString();
            }
            return s;
        }

        private string setURL(string url)
        {
            string newurl = "";
            newurl = url.Replace("../", "");
            newurl = newurl.Replace("..", "");
            if (newurl.Substring(0, 1) != "/")
            {
                return "/" + newurl;
            }
            else
            {
                return newurl;
            }
        }

        private DataTable GetList(string sId,string path, string date)
        {
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("Path",path),
                new MySqlParameter("Date",date)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_userView1", mpara);

            return dt;
        }

        private DataTable GetList2(string sId, string date)
        {
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("Date",date)
            };
            DataTable dt = db.ExecProcedure("slsoft_ias_bus_p_stat_userView2", mpara);

            return dt;
        }
    }
}
