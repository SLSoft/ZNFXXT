using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using SLSoft.EasyORM;
using MySql.Data.MySqlClient;

namespace SLSoft.ResidentProgram.Controllers
{
    public class ContrastByDayController : Controller
    {
        //
        // GET: /ContrastByDay/

        public string Index()
        {
            string strJson = "";

            if (Request.QueryString["sId"] != null && Request.QueryString["startDate"] != null && Request.QueryString["endDate"] != null && Request.QueryString["beginDate"] != null && Request.QueryString["overDate"] != null)
            {
                string sId = Request.QueryString["sId"].ToString();
                string startDate = Request.QueryString["startDate"].ToString();
                string endDate = Request.QueryString["endDate"].ToString();
                string beginDate = Request.QueryString["beginDate"].ToString();
                string overDate = Request.QueryString["overDate"].ToString();

                DataTable dt = GetList(sId, startDate, endDate);
                DataTable dtCon = GetList(sId, beginDate, overDate);

                if (dt != null && dtCon != null)
                {
                    strJson = Common.JsonHelper.ToJson(UniteDataTable(dt, dtCon));
                }
            }
            string callback = HttpContext.Request["Callback"];
            return callback + "(" + strJson + ")";
        }

        private DataTable GetList(string sId, string startDate, string endDate)
        {
            DBFactory df = new DBFactory();
            AbstractDB db = null;
            db = df.CreateDB("mysql");

            MySqlParameter[] mpara ={
                new MySqlParameter("SiteID", sId),
                new MySqlParameter("startDate", startDate),
                new MySqlParameter("endDate",endDate)
            };
            return db.ExecProcedure("slsoft_ias_bus_p_stat_ContrastByDay", mpara);
        }

        private DataTable UniteDataTable(DataTable dt1, DataTable dt2)
        {
            DataTable dt3 = dt1.Clone();

            for (int i = 1; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add("C" + dt2.Columns[i].ColumnName);
            }
            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }

            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 1; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count - 1] = dt2.Rows[i][j].ToString();
                    }
                    dt3.Rows[i][0] = string.Format("{0} VS {1}", dt1.Rows[i][0].ToString(),dt2.Rows[i][0].ToString());
                }
                if (dt1.Rows.Count > dt2.Rows.Count)
                {
                    for (int r = dt1.Rows.Count - dt2.Rows.Count - 1; r < dt3.Rows.Count; r++)
                    {
                        if (dt3.Rows[r][0].ToString().IndexOf("VS") <= -1)
                        {
                            dt3.Rows[r][0] = string.Format("{0} VS {1}", dt3.Rows[r][0].ToString(), "-");
                        }
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    dr3[0] = "";
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 1; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count - 1] = dt2.Rows[i][j].ToString();
                    }
                    dt3.Rows[i][0] = dt3.Rows[i][0].ToString() == "" ? string.Format("{0} VS {1}", "-", dt2.Rows[i][0].ToString()) : string.Format("{0} VS {1}", dt3.Rows[i][0].ToString(), dt2.Rows[i][0].ToString());
                }
            }
            return dt3;
        }

    }
}
