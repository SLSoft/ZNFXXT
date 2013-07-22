using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;

namespace SLSoft.EasyORM
{
    public class DBMySql : AbstractDB
    {
        private string conn;

        public DBMySql(string cs)
        {
            base.New(cs);
            conn = cs;
        }

        public override DataTable ExecSql(string strSql)
        {
            return MySqlHelper.ExecuteDataSet(conn, CommandType.Text, strSql,null).Tables[0];
        }

        public override int ExecSqlNoquery(string strSql)
        {
            return MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, strSql, null);
        }

        public override DataTable ExecProcedure(string strName, params MySqlParameter[] p)
        {
            return MySqlHelper.ExecuteDataSet(conn, CommandType.StoredProcedure, strName, p).Tables[0];
        }

        public override int ExecProNoquery(string strName, params MySqlParameter[] p)
        {
            return MySqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, strName, p);
        }
        public override DataTable ExecProcedure(string strName,SqlParameter[] p)
        {
            return null;
        }
        public override int ExecProNoquery(string strName, SqlParameter[] p)
        {
            return 0;
        }
    }
}
