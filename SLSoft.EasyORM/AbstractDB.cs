using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace SLSoft.EasyORM
{
    public abstract class AbstractDB
    {
        protected string connString;
        public string ConnString
        {
            get { return connString; }
            set { connString = value; }
        }

        public void New(string cs)
        {
            connString = cs;
        }

        public abstract DataTable ExecSql(string strSql);
        public abstract int ExecSqlNoquery(string strSql);
        public abstract DataTable ExecProcedure(string strName, params MySqlParameter[] p);
        public abstract int ExecProNoquery(string strName, params MySqlParameter[] p);
        public abstract DataTable ExecProcedure(string strName, SqlParameter[] p);
        public abstract int ExecProNoquery(string strName, SqlParameter[] p);
    }
}
