﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LitJson;
using SLSoft.ResidentProgram.Models;

namespace SLSoft.ResidentProgram.Controllers
{
    public class StartController : Controller
    {
        //
        // GET: /Start/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Context()
        {
            return View();
        }
        
        public ActionResult GetData(FormCollection fc)
        {
            

            Information im = new Information();
            im.IsMove = fc["isMove"];//是否移动终端
            im.MoveType = fc["moveType"];//移动终端类型
            im.BrowserType = fc["browserType"];//浏览器类型
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
            im.ParentUrl = fc["parentUrl"];//父窗口URL
            im.ExpTime = fc["expTime"];//cookie过期时间

            im.UserHostName = System.Net.Dns.GetHostName();//客户端计算机名
            im.UserHostAddress = Request.UserHostAddress;//客户端IP
            im.ServerAddress = Request.ServerVariables["LOCAL_ADDR"];//服务器端IP
            im.JavaApplets = Request.Browser.JavaApplets;//是否支持JAVA
            im.Frames = Request.Browser.Frames;//是否支持框架网页


            return Json(new { });
        }

        //添加信息到访问表
        private void Insert_T_Session(Information im)
        { 
            
        }

        //添加信息到访问明细表
        private void Insert_T_Session(Information im)
        { 
        
        }

        //是否同一次访问
        private bool IsSameVisited(string sessionid)
        {
            return false;
        }
    }
}
