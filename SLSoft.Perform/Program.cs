using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLSoft.EasyORM;
using System.Data;
using MySql.Data.MySqlClient;

namespace SLSoft.Perform
{
    public class Program
    {
        private static string conn = System.Configuration.ConfigurationSettings.AppSettings["strConn"].ToString();
        private static string sDate = DateTime.Now.ToString("yyyy-MM-dd");
        //private static string sDate = Convert.ToDateTime("2013-10-17").ToString("yyyy-MM-dd");
        private static DataTable dtColumn = GetColumn();
        private static DataTable dtFirstColumn = GetFirstColumn();
        private static DataTable dtList = GetList();

        private static int cid = 0;
        private static string cname = "";
        private static int fid = 0;
        private static int pvnum = 0;
        private static int uvnum = 0;
        private static int ipnum = 0;
        private static int lennum = 0;
        private static int unum = 0;

        static void Main(string[] args)
        {
            WriteColumnClickList();
            WriteFirstList();
            WriteSecondList();
        }

        //按栏目点击量统计定时写入
        private static void WriteColumnClickList()
        {
            try
            {
                MySqlParameter[] mpara ={
                    new MySqlParameter("sDate",sDate)
                };
                SLSoft.EasyORM.MySqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "slsoft_ias_bus_p_timing_site1_columnClick", mpara);
                System.Console.Write("写入成功");
            }
            catch (Exception ex)
            {
                System.Console.Write(ex.Message);

            }
        }
        //按栏目浏览量统计写入（一级栏目）
        private static void WriteFirstList()
        {
            for (int i = 1; i < dtFirstColumn.Rows.Count; i++)
            {
                GetInfo(dtFirstColumn.Rows[i]["ID"].ToString());

                cid = int.Parse(dtFirstColumn.Rows[i]["ID"].ToString());
                cname = dtFirstColumn.Rows[i]["CrunodeName"].ToString();
                fid = int.Parse(dtFirstColumn.Rows[i]["FatherID"].ToString());

                WriteColumn();
            }
        }
        //按栏目浏览量统计写入（二级栏目）
        private static void WriteSecondList()
        {
            for (int i = 1; i < dtFirstColumn.Rows.Count; i++)
            {
                //根据一级菜单获取二级菜单
                string strSql = string.Format("select * from slsoft_ias_dic_t_column where FatherID = {0}", dtFirstColumn.Rows[i]["ID"].ToString());
                DataTable dt = SLSoft.EasyORM.MySqlHelper.ExecuteDataSet(conn, CommandType.Text, strSql, null).Tables[0];

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    GetInfo(dt.Rows[j]["ID"].ToString());

                    cid = int.Parse(dt.Rows[j]["ID"].ToString());
                    cname = dt.Rows[j]["CrunodeName"].ToString();
                    fid = int.Parse(dt.Rows[j]["FatherID"].ToString());

                    WriteColumn();
                }
            }
        }

        //根据id获取子级浏览数
        private static void GetInfo(string id)
        {
            DataTable dtnew = dtList.Clone();

            string strFatherID = GetFatherID(id, dtColumn);
            DataRow[] rows = dtList.Select(string.Format("pathID in ({0})", strFatherID));

            foreach (DataRow row in rows)
            {
                dtnew.ImportRow(row);
            }

            if (rows.Count() > 0)
            {
                pvnum = int.Parse(dtnew.Compute("sum(pvnum)", "true").ToString());
                uvnum = int.Parse(dtnew.Compute("sum(uvnum)", "true").ToString());
                ipnum = int.Parse(dtnew.Compute("sum(ipnum)", "true").ToString());
                lennum = int.Parse(dtnew.Compute("sum(lennum)", "true").ToString());
                unum = int.Parse(dtnew.Compute("sum(unum)", "true").ToString());
            }
            else
            {
                SetNum();
            }
        }

        //递归获取父级子级ID
        private static string GetFatherID(string pid, DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(pid);
            DataRow[] rows = dt.Select("FatherID=" + pid);
            foreach (DataRow dr in rows)
            {
                sb.Append(",");
                sb.Append(GetFatherID(dr["ID"].ToString(), dt));  //递归
            }
            return sb.ToString();
        }

        //获取所有栏目
        private static DataTable GetColumn()
        {
            string strSql = "select * from slsoft_ias_dic_t_column";
            DataTable dt = SLSoft.EasyORM.MySqlHelper.ExecuteDataSet(conn, CommandType.Text, strSql, null).Tables[0];
            return dt;
        }

        //获取一级栏目
        private static DataTable GetFirstColumn()
        {
            string strSql = "select * from slsoft_ias_dic_t_column where FatherID = 0";
            DataTable dt = SLSoft.EasyORM.MySqlHelper.ExecuteDataSet(conn, CommandType.Text, strSql, null).Tables[0];
            return dt;

        }

        //获取当天访问明细数据
        private static DataTable GetList()
        {
            MySqlParameter[] mpara ={
                    new MySqlParameter("sDate",sDate)
                };
            return SLSoft.EasyORM.MySqlHelper.ExecuteDataSet(conn, CommandType.StoredProcedure, "slsoft_ias_bus_p_timing_site1_column", mpara).Tables[0];
        }

        //写入栏目浏览统计表
        private static void WriteColumn()
        {
            try
            {
                MySqlParameter[] mpara ={
                    new MySqlParameter("cid",cid),
                    new MySqlParameter("cname",cname),
                    new MySqlParameter("fid",fid),
                    new MySqlParameter("pvnum",pvnum),
                    new MySqlParameter("uvnum",uvnum),
                    new MySqlParameter("ipnum",ipnum),
                    new MySqlParameter("lennum",lennum),
                    new MySqlParameter("unum",unum),
                    new MySqlParameter("CreateTime",sDate)
                };
                SLSoft.EasyORM.MySqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "slsoft_ias_bus_p_insert_columnTemp", mpara);
            }
            catch (Exception ex)
            {
                System.Console.Write(ex.Message);

            }
        }

        private static void SetNum()
        {
            pvnum = 0;
            uvnum = 0;
            ipnum = 0;
            lennum = 0;
            uvnum = 0;
        }

        #region

        ////按栏目浏览量统计（一级栏目）定时写入
        //private static void WriteFirstList()
        //{
        //    WriteColumnIndexPV(sDate, eDate);

        //    for (int i = 1; i < dtFirstColumn.Rows.Count; i++)
        //    {
        //        WriteColumnPV(dtFirstColumn.Rows[i]["ID"].ToString(), dtFirstColumn.Rows[i]["FatherID"].ToString(), dtFirstColumn.Rows[i]["CrunodeName"].ToString(), sDate, eDate);
        //    }
        //}

        ////按栏目浏览量统计（二级栏目）定时写入
        //private static void WriteSecondList()
        //{
        //    for (int i = 1; i < dtFirstColumn.Rows.Count; i++)
        //    {
        //        //根据一级菜单获取二级菜单
        //        string strSql = string.Format("select * from slsoft_ias_dic_t_column where FatherID = {0}", dtFirstColumn.Rows[i]["ID"].ToString());
        //        DataTable dt = SLSoft.EasyORM.MySqlHelper.ExecuteDataSet(conn, CommandType.Text, strSql, null).Tables[0];

        //        for (int j = 0; j < dt.Rows.Count; j++)
        //        {
        //            WriteColumnPV(dt.Rows[j]["ID"].ToString(), dt.Rows[j]["FatherID"].ToString(), dt.Rows[j]["CrunodeName"].ToString(), sDate, eDate);
        //        }
        //    }
        //}
        ////写入首页pv浏览量
        //private static void WriteColumnIndexPV(string sdate, string edate)
        //{
        //    try
        //    {
        //        MySqlParameter[] mpara ={
        //                    new MySqlParameter("sDate",sdate),
        //                    new MySqlParameter("eDate",edate)
        //                };
        //        SLSoft.EasyORM.MySqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "slsoft_ias_bus_p_timing_site1_columnIndexPV", mpara);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.Write(ex.Message);

        //    }
        //}
        ////写入首页以外的一级菜单和二级菜单总浏览量
        //private static void WriteColumnPV(string id, string fid, string cname, string sdate, string edate)
        //{
        //    try
        //    {
        //        MySqlParameter[] mpara ={
        //                    new MySqlParameter("c_ID",id),
        //                    new MySqlParameter("f_ID",fid),
        //                    new MySqlParameter("cName",cname),
        //                    new MySqlParameter("sDate",sdate),
        //                    new MySqlParameter("eDate",edate)
        //                };
        //        SLSoft.EasyORM.MySqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "slsoft_ias_bus_p_timing_site1_columnPV", mpara);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Console.Write(ex.Message);

        //    }
        //}

        #endregion
    }
}
