(function () {
    var isMove = false; //是否移动终端
    var moveType = ""; //移动终端类型
    var browserType = ""; //浏览器类型
    var bVersions = ""; //浏览器版本

    getBrowser();
    chckeMove();
    var bLanguage = navigator.language; //浏览器语言
    var sysLanguage = navigator.systemLanguage; //系统语言
    var userLanguage = navigator.userLanguage; //用户语言
    var cpuType = navigator.cpuClass; //CPU类型，不支持火狐
    var OS = getOS(); //操作系统
    var size = screen.width + "*" + screen.height; //分辨率
    var isCookie = CookieEnable(); //是否支持cookie
    var plugins = getPlugins(); //插件 不支持IE
    var vColor = ""; //色彩
    var vDate = new Date();
    var zone = (0 - vDate.getTimezoneOffset() / 60); //时区
    var pageUpUrl = document.referrer; //上一页URL
    var currentName = window.location.host; //当前域名
    var currentUrl = window.location; //当前URL
    var parentUrl = window.parent.location; //父窗口URL
    var expTime = 1; //cookie过期时间

    if (!bLanguage) {//判断IE浏览器使用语言
        bLanguage = navigator.browserLanguage;
    }
    if (navigator.appName == "Netscape") { vColor = screen.pixelDepth; } else { vColor = screen.colorDepth; }

    var strJSON = '{"isMove":"' + isMove + '","moveType":"' + moveType + '","browserType":"' + browserType + '","bVersions":"' + bVersions + '","bLanguage":"' + bLanguage + '","sysLanguage":"' + sysLanguage + '","userLanguage":"' + userLanguage + '","cpuType":"' + cpuType + '","OS":"' + OS + '","size":"' + size + '","isCookie":"' + isCookie + '","plugins":"' + plugins + '","vColor":"' + vColor + '","zone":"' + zone + '","pageUpUrl":"' + pageUpUrl + '","currentName":"' + currentName + '","currentUrl":"' + currentUrl + '","parentUrl":"' + parentUrl + '","expTime":"' + expTime + '"}';
    var myData = jQuery.parseJSON(strJSON);


    $.ajax({
        type: "POST",
        url: "/Start/GetData",
        data: myData,
        dataType: "json",
        success: function (res) {
            //                if (res.success) {
            //                    alert("操作成功");
            //                } else {
            //                    alert("操作失败 ：" + res.message);
            //                }
        },
        error: function () {
            alert("发生错误。");
        }
    });


    function chckeMove() {
        var browser = {
            versions: function () {
                var u = navigator.userAgent, app = navigator.appVersion;
                return {         //移动终端浏览器版本信息
                    trident: u.indexOf('Trident') > -1, //IE内核
                    presto: u.indexOf('Presto') > -1, //opera内核
                    webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                    gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1, //火狐内核
                    mobile: !!u.match(/AppleWebKit.*Mobile.*/), //是否为移动终端
                    ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                    android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或uc浏览器
                    iPhone: u.indexOf('iPhone') > -1, //是否为iPhone或者QQHD浏览器
                    iPad: u.indexOf('iPad') > -1, //是否iPad
                    webApp: u.indexOf('Safari') == -1 //是否web应该程序，没有头部与底部
                };
            } (),
            language: (navigator.browserLanguage || navigator.language).toLowerCase()
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
    //是否支持cookie
    function CookieEnable() {
        var result = "不支持";
        if (navigator.cookiesEnabled)
            return "支持";
        document.cookie = "testcookie=yes;";
        var cookieSet = document.cookie;
        if (cookieSet.indexOf("testcookie=yes") > -1)
            result = "支持";
        document.cookie = "";
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
            filename = navigator.plugins[i].filename;
            str += name + "---->" + filename + "<br/>";
        }
        return str;
    }
})();