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
function SetCookie(name, value) {
    var days = 30; //保存30天
    var exp = new Date();
    exp.setTime(exp.getTime() + days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";path=/;expires=" + exp.toGMTString();
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
        if (browser.versions.ios) { moveType = "ios终端"; }
        else if (browser.versions.android) { moveType = "android"; }
        else if (browser.versions.iPhone) { moveType = "iPhone"; }
        else if (browser.versions.iPad) { moveType = "iPad"; }
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

/*获取浏览器所有插件*/
function getPlugins() {
    //取得插件的个数
    num = navigator.plugins.length;
    var str = "";
    for (i = 0; i < num; i++) {
        //取得插件的名称
        name = navigator.plugins[i].name;
        //取得插件的文件名称
        //filename = navigator.plugins[i].filename;
        //str += name + "---->" + filename + "<br/>";
        str += name + " ";
    }
    return str;
}