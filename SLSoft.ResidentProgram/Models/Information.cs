﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SLSoft.ResidentProgram.Models
{
    public class Information
    {
        public Information()
        { 
        
        }
        /// <summary>
        /// js数据
        /// </summary>
        public string sId { set; get; }//站点ID
        public string IsMove { set; get; }//是否移动终端
        public string MoveType { set; get; }//移动终端类型
        public string BrowserType { set; get; }//浏览器类型
        public string BrowserKernel { set; get; }//浏览器内核
        public string BVersions { set; get; }//浏览器版本
        public string BLanguage { set; get; }//浏览器语言
        public string SysLanguage { set; get; }//系统语言
        public string UserLanguage { set; get; }//用户语言
        public string CpuType { set; get; }//CPU类型
        public string OS { set; get; }//操作系统
        public string Size { set; get; }//分辨率
        public string IsCookie { set; get; }//是否支持cookie
        public string Plugins { set; get; }//插件
        public string VColor { set; get; }//色彩
        public string Zone { set; get; }//时区
        public string PageUpUrl { set; get; }//上一页URL
        public string CurrentName { set; get; }//当前域名
        public string CurrentUrl { set; get; }//当前URL
        public string currentUrlTitle { set; get; }//当前URL标题
        public string ParentUrl { set; get; }//父窗口URL
        public string ClientX { set; get; }//X坐标
        public string ClientY { set; get; }//Y坐标

        /// <summary>
        /// cookie数据
        /// </summary>
        public string UserCode { set; get; }//用户唯一标识
        public string SessionCode { set; get; }//cookie过期时间
        public string SessionType { set; get; }//是否IsUV
        public string ApplicationType { set; get; }//是否IsNewUV
        public string AccessCount { set; get; }//当日访问次数
        public string LastAccessTime { set; get; }//上一次访问时间
        public string OperType { set; get; }//操作类型

        /// <summary>
        /// 服务器数据
        /// </summary>
        public string UserHostName { set; get; }//客户端计算机名
        public string UserHostAddress { set; get; }//客户端IP
        public string ServerAddress { set; get; }//服务器端IP
        public bool JavaApplets { set; get; }//是否支持JAVA
        public bool Frames { set; get; }//是否支持框架网页

    }
}