using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

namespace SLSoft.EasyORM
{
    public class DBFactory
    {
        public AbstractDB CreateDB(string strType)
        {
            AbstractDB db = null;
            string strConn = "";

            string strConfigPath = System.Configuration.ConfigurationManager.AppSettings.Get("EasyORMConfigPath");
            string Path = System.Web.HttpContext.Current.Server.MapPath("~")+ strConfigPath;
            
            XmlDocument xml = new XmlDocument();
            xml.Load(Path);
            XmlNode node = null;
            XmlNodeList xnl = null;

            switch (strType)
            {
                case "mysql":
                    node = xml.SelectSingleNode("database[@modle='single']");
                    xnl = getList(node);

                    strConn = string.Format("Server={0};Uid={1};Pwd={2};Database={3};", xnl.Item(1).InnerText, xnl.Item(2).InnerText, xnl.Item(3).InnerText, xnl.Item(4).InnerText);
                    db = new DBMySql(strConn);
                    break;
                case "sql":
                    node = xml.SelectSingleNode("database[@modle='single']");
                    xnl = getList(node);

                    strConn = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3};", xnl.Item(1).InnerText, xnl.Item(2).InnerText, xnl.Item(3).InnerText, xnl.Item(4).InnerText);                            
                    db = new DBSql(strConn);
                    break;
                case "oracle":
                    //mysql = new OracleDB(strConnString);
                    break;
            }
            return db;
        }

        public XmlNodeList getList(XmlNode node)
        {
            XmlNodeList xnl = null;
            if (node != null)
            {
                XmlNodeList childlist = node.ChildNodes;
                foreach (XmlNode xnf in childlist)
                {
                    XmlElement xe = (XmlElement)xnf;
                    xnl = xe.ChildNodes;
                }
            }
            return xnl;
        }
    }
}
