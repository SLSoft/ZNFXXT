(function () {
    window.SL_jsonpHandle = function (a) {
        var isMove = false; //是否移动终端
        var moveType = ""; //移动终端类型
        var browserType = ""; //浏览器类型
        var bVersions = ""; //浏览器版本
        getBrowser();
        chckeMove();
        var IP = a.host;
        var bLanguage = navigator.language; //浏览器语言
        var browserKernel = getBrowserKernel(); //浏览器内核
        var sysLanguage = navigator.systemLanguage; //系统语言
        var userLanguage = navigator.userLanguage; //用户语言
        var cpuType = navigator.cpuClass; //CPU类型，不支持火狐
        var OS = getOS(); //操作系统
        var size = screen.width + "*" + screen.height; //分辨率
        var isCookie = CookieEnable(); //是否支持cookie
        var plugins = ""; //插件 不支持IE
        var vColor = ""; //色彩
        var vDate = new Date();
        var zone = (0 - vDate.getTimezoneOffset() / 60); //时区
        var pageUpUrl = decodeURIComponent(document.referrer); //上一页URL
        var currentName = window.location.host; //当前域名
        var currentUrl = window.location; //当前URL
        var currentUrlTitle = document.title; //当前URL标题
        var parentUrl = window.parent.location; //父窗口URL
        var clientX = 0;
        var clientY = 0;

        var sId = GetSiteId();
        if (!sId) { sId = 0; }

        var obj = document.documentElement;
        obj.onclick = function (e) {
            oEvent = e || event;
            clientX = oEvent.clientX;
            clientY = oEvent.clientY;

            var xycookie = "x=" + clientX + "&y=" + clientY;
            setCookie("SLSoft_XY_Position_" + sId, xycookie, 4);
        };
        if (!CheckCookie("SLSoft_XY_Position_" + sId)) {
            var xyvalues = GetCookie("SLSoft_XY_Position_" + sId);
            clientX = GetValue(xyvalues, "x");
            clientY = GetValue(xyvalues, "y");
        }

        if (!bLanguage) { bLanguage = navigator.browserLanguage; }
        if (!sysLanguage) { sysLanguage = navigator.language; }
        if (!userLanguage) { userLanguage = navigator.language; }
        if (!cpuType) { cpuType = "x86"; }
        if (navigator.appName == "Netscape") { vColor = screen.pixelDepth; } else { vColor = screen.colorDepth; }
        if (!clientX) { clientX = 0; }
        if (!clientY) { clientY = 0; }

        var cookieId = ''; //用户唯一标识
        var session = ''; //session会话
        var sessiontype = 0; //是否独立访客
        var apptype = 0; //是否新的独立访客(0:否 1:是)
        var count = 0; //当日访问次数
        var totalcount = 0; //总访问次数
        var otype = 0; //后台数据库操作

        //用户第一次访问设置为新用户
        if (CheckCookie("SLSoft_IA_USER_" + sId)) {
            apptype = 1;
            cookieId = newGuid();
            totalcount = 1;
        }
        else {
            var values = GetCookie("SLSoft_IA_USER_" + sId);
            cookieId = GetValue(values, "usercode");
            totalcount = GetValue(values, "totalcount");

            if (CheckCookie("SLSoft_IA_SESSION" + sId)) {
                totalcount = parseInt(totalcount) + 1;
            }
        }
        var usercookie = "usercode=" + cookieId + "&apptype=" + apptype + "&totalcount=" + totalcount;
        setCookie("SLSoft_IA_USER_" + sId, usercookie, 1); //有效期2年

        if (!CheckCookie("SLSoft_IA_SESSION" + sId)) {//已存在
            otype = 1; //操作数据库类型
            session = GetCookie("SLSoft_IA_SESSION" + sId); //获取session值
            var values = GetCookie("SLSoft_IA_TYPE_" + sId);
            count = GetValue(values, "count");
        }
        else {
            session = newGuid();
            if (!CheckCookie("SLSoft_IA_TYPE_" + sId)) {
                var values = GetCookie("SLSoft_IA_TYPE_" + sId);
                count = GetValue(values, "count");
                count = parseInt(count) + 1;
            }
            else {
                count += 1;
                sessiontype = 1;
            }
        }
        var typecookie = "count=" + count + "&sessiontype=" + sessiontype;
        setCookie("SLSoft_IA_TYPE_" + sId, typecookie, 3);  //有效期当天
        setCookie("SLSoft_IA_SESSION" + sId, session, 2); //有效期30分钟

        var cookie = "userCode=" + cookieId + "&session=" + session + "&stype=" + sessiontype + "&atype=" + apptype + "&otype=" + otype + "&count=" + count;
        var data = "sId=" + sId + "&isMove=" + getEncode(isMove) + "&moveType=" + getEncode(moveType) + "&browserType=" + getEncode(browserType) + "&browserKernel=" + getEncode(browserKernel) + "&bVersions=" + bVersions + "&bLanguage=" + bLanguage + "&sysLanguage=" + sysLanguage + "&userLanguage=" + userLanguage + "&cpuType=" + cpuType + "&OS=" + OS + "&size=" + size + "&isCookie=" + isCookie + "&plugins=" + getEncode(plugins) + "&vColor=" + vColor + "&zone=" + zone + "&pageUpUrl=" + getEncode(pageUpUrl) + "&cName=" + getEncode(currentName) + "&cUrl=" + currentUrl + "&parentUrl=" + parentUrl + "&cUrlTitle=" + getEncode(currentUrlTitle) + "&clientX=" + clientX + "&clientY=" + clientY + "&IP=" + IP + "&" + cookie;

        if (browserType == "IE") {
            var xdr = new XDomainRequest();
            xdr.open("POST", "/Start/GetData");
            xdr.send(data);
        }
        else {

            var xmlHttpRequest = getXmlHttpRequest();
            if (xmlHttpRequest == null) return false;
            xmlHttpRequest.open("POST", "/Start/GetData", true);
            //xmlHttpRequest.setRequestHeader("CONTENT-TYPE", "application/x-www-form-urlencoded");
            xmlHttpRequest.send(data);
        }

        function getXmlHttpRequest() {
            var xmlHttpRequest = null;
            try {
                xmlHttpRequest = new ActiveXObject("Msxml2.XMLHTTP");
            }
            catch (e1) {
                try {
                    xmlHttpRequest = new ActiveXObject("Microsoft.XMLHTTP");
                } catch (e2) {
                    xmlHttpRequest = null;
                }
            }
            if (xmlHttpRequest == null && typeof (XMLHttpRequest) != 'undefined') {
                xmlHttpRequest = new XMLHttpRequest();
            }
            return xmlHttpRequest;
        }

        function getEncode(arguments) {
            return encodeURIComponent(arguments);
        }

        function GetValue(c, n) {
            var values = c.split(/[\?&]/);
            var tmpStr = n + "=";
            for (var i = 0; i < values.length; i++)
                if (values[i].search(eval("/^" + tmpStr + "/i")) != -1) return values[i].substring(tmpStr.length);
        }
        //写Cookies
        function setCookie(name, value, time) {
            var exp = new Date();
            if (time == 1) {
                exp.setTime(exp.getTime() + 2 * 365 * 24 * 60 * 60 * 1000);
            }
            else if (time == 2) {
                exp.setMinutes(exp.getMinutes() + 30);
            }
            else if (time == 3) {
                exp.setDate(exp.getDate() + 1);
                exp.setHours(0);
                exp.setMinutes(0);
                exp.setSeconds(0);
            }
            else if (time == 4) {
                exp.setSeconds(exp.getSeconds() + 10);
            }
            document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
        }
        //读cookies
        function GetCookie(sName) {
            var sSearch = sName + "=";
            if (document.cookie.length > 0) {
                offset = document.cookie.indexOf(sSearch)
                if (offset != -1) {
                    offset += sSearch.length;
                    end = document.cookie.indexOf(";", offset)
                    if (end == -1) end = document.cookie.length;
                    return unescape(document.cookie.substring(offset, end))
                }
                else return ""
            }
        }

        //检查cookie
        function CheckCookie(name) {
            var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");
            if (arr = document.cookie.match(reg)) {
                return false; //已经登录过
            }
            else {
                return true; //没有登录
            }
        }

        function newGuid() {
            var guid = "";
            for (var i = 1; i <= 32; i++) {
                var n = Math.floor(Math.random() * 16.0).toString(16);
                guid += n;
                if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                    guid += "-";
            }
            return guid;
        }

        //是否移动终端
        function chckeMove() {
            var browser = {
                versions: function () {
                    var u = navigator.userAgent, app = navigator.appVersion;
                    return {         //移动终端浏览器版本信息
                        mobile: !!u.match(/AppleWebKit.*Mobile.*/), //是否为移动终端
                        ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                        android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或uc浏览器
                        iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器
                        iPad: u.indexOf('iPad') > -1, //是否iPad
                        webApp: u.indexOf('Safari') == -1 //是否web应该程序，没有头部与底部
                    };
                } ()
            }
            if (browser.versions.mobile) {
                isMove = "是";
                if (browser.versions.android) { moveType = "android"; }
                else if (browser.versions.iPhone) { moveType = "iPhone"; }
                else if (browser.versions.iPad) { moveType = "iPad"; }
                else if (browser.versions.ios) { moveType = "ios终端"; }
                else if (browser.versions.webApp) { moveType = "Safari"; }
                else { moveType = "其他"; }
            } else { isMove = "否"; }
        }

        //获取浏览器内核
        function getBrowserKernel() {
            var browser = {
                versions: function () {
                    var u = navigator.userAgent, app = navigator.appVersion;
                    return {         //移动终端浏览器版本信息
                        trident: u.indexOf('Trident') > -1, //IE内核
                        presto: u.indexOf('Presto') > -1, //opera内核
                        webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                        gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1 //火狐内核
                    };
                } ()
            }
            if (browser.versions.trident) { return "IE内核:Trident"; }
            if (browser.versions.presto) { return "opera内核:Presto"; }
            if (browser.versions.webKit) { return "苹果、谷歌内核:AppleWebKit"; }
            if (browser.versions.gecko) { return "火狐内核:Gecko"; }
            return "其他";
        }

        //获取浏览器类型、版本
        function getBrowser() {
            var Sys = {};
            var ua = navigator.userAgent.toLowerCase();
            var s;
            (s = ua.match(/msie ([\d.]+)/)) ? Sys.ie = s[1] :
            (s = ua.match(/firefox\/([\d.]+)/)) ? Sys.firefox = s[1] :
            (s = ua.match(/chrome\/([\d.]+)/)) ? Sys.chrome = s[1] :
            (s = ua.match(/opera.([\d.]+)/)) ? Sys.opera = s[1] :
            (s = ua.match(/version\/([\d.]+).*safari/)) ? Sys.safari = s[1] : 0;

            if (Sys.ie) { browserType = "IE"; bVersions = Sys.ie; }
            if (Sys.firefox) { browserType = "Firefox"; bVersions = Sys.firefox; }
            if (Sys.chrome) { browserType = "Chrome"; bVersions = Sys.chrome; }
            if (Sys.opera) { browserType = "Opera"; bVersions = Sys.opera; }
            if (Sys.safari) { browserType = "Safari"; bVersions = Sys.safari; }
        }

        //获取操作系统
        function getOS() {
            var sUserAgent = navigator.userAgent;
            var isWin = (navigator.platform == "Win32") || (navigator.platform == "Windows");
            var isMac = (navigator.platform == "Mac68K") || (navigator.platform == "MacPPC") || (navigator.platform == "Macintosh") || (navigator.platform == "MacIntel");
            if (isMac) return "Mac";
            var isUnix = (navigator.platform == "X11") && !isWin && !isMac;
            if (isUnix) return "Unix";
            var isLinux = (String(navigator.platform).indexOf("Linux") > -1);
            if (isLinux) return "Linux";
            if (isWin) {
                var isWin2K = sUserAgent.indexOf("Windows NT 5.0") > -1 || sUserAgent.indexOf("Windows 2000") > -1;
                if (isWin2K) return "Win2000";
                var isWinXP = sUserAgent.indexOf("Windows NT 5.1") > -1 || sUserAgent.indexOf("Windows XP") > -1;
                if (isWinXP) return "WinXP";
                var isWin2003 = sUserAgent.indexOf("Windows NT 5.2") > -1 || sUserAgent.indexOf("Windows 2003") > -1;
                if (isWin2003) return "Win2003";
                var isWinVista = sUserAgent.indexOf("Windows NT 6.0") > -1 || sUserAgent.indexOf("Windows Vista") > -1;
                if (isWinVista) return "WinVista";
                var isWin7 = sUserAgent.indexOf("Windows NT 6.1") > -1 || sUserAgent.indexOf("Windows 7") > -1;
                if (isWin7) return "Win7";
            }
            return "other";
        }
        //是否支持cookie  0:不支持 1:支持
        function CookieEnable() {
            var result = 0;
            if (navigator.cookiesEnabled)
                return 1;

            document.cookie = "testcookie=yes";
            var cookieSet = document.cookie;
            if (cookieSet.indexOf("testcookie=yes") > -1)
                result = 1;

            var date = new Date();
            date.setTime(date.getTime() - 10000);
            document.cookie = "testcookie=yes; expires=" + date.toGMTString();

            return result;
        }

        function GetSiteId() {
            var arr;
            var reg = /(?:^|\?|&)sId=(.+?)(?:&|$)/;
            arr = document.getElementsByTagName("body")[0].innerHTML.match(reg);
            if (arr != null) {
                return arr[1];
            }
            return 0;
        }

    }
    var JSONP = document.createElement("script");
    JSONP.type = "text/javascript";
    JSONP.src = "http://smart-ip.net/geoip-json?callback=SL_jsonpHandle";
    document.getElementsByTagName("body")[0].appendChild(JSONP);

})();

