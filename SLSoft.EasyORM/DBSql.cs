using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace SLSoft.EasyORM
{
    public class DBSql : AbstractDB
    {
        private SqlConnection conn;

        public DBSql(string cs)
        {
            base.New(cs);
            conn = new SqlConnection(cs);
        }

        public override DataTable ExecSql(string strSql)
        {
            return SqlHelper.ExecuteDataTable(conn, CommandType.Text, strSql);
        }

        public override int ExecSqlNoquery(string strSql)
        {
            return SqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql);
        }

        public override DataTable ExecProcedure(string strName, SqlParameter[] p)
        {
            return SqlHelper.ExecuteDataTable(conn, CommandType.StoredProcedure, strName, p);
        }

        public override int ExecProNoquery(string strName, SqlParameter[] p)
        {
            return SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, strName, p);
        }

        public override DataTable ExecProcedure(string strName, params MySqlParameter[] p)
        {
            return null;
        }

        public override int ExecProNoquery(string strName, params MySqlParameter[] p)
        {
            return 0;
        }
        public override object ExecuteScalar(string strName, params MySqlParameter[] p)
        {
            return null;
        }

        public override DataSet ExecProcedureDateSet(string strName, params MySqlParameter[] p)
        {
            return null;
        }
    }
}
