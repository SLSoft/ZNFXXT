using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Data.OracleClient;

namespace SLSoft.EasyORM
{
    /// <summary>
    /// 针对Oracle数据库操作的通用类
    /// </summary>
    public class OracleDbHelper
    {
        private string connectionString;
        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            set { connectionString = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public OracleDbHelper()
        {
            //#warning 注意，如果采用这种方式构建实例，必须在web.config中配置“conn”的数据库连接字符串
            //connectionString = ConfigurationManager.ConnectionStrings["OracleConnectionString"].ConnectionString;
            //connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public OracleDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// 执行一个查询，并返回结果集
        /// </summary>
        /// <param name="commandText">要执行的查询Oracle文本命令</param>
        /// <returns>返回查询结果集</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// 执行一个查询,并返回查询结果
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <returns>返回查询结果集</returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return ExecuteDataTable(commandText, commandType, null);
        }
        /// <summary>
        /// 执行一个查询,并返回查询结果
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //通过包含查询Oracle的OracleCommand实例来实例化OracleDataAdapter
                    OracleDataAdapter adapter = new OracleDataAdapter(command);

                    adapter.Fill(data);//填充DataTable
                }
            }
            return data;
        }
        /// <summary>
        /// 将 CommandText 发送到 Connection 并生成一个 OracleDataAdapter。
        /// </summary>
        /// <param name="commandText">要执行的查询Oracle文本命令</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// 将 CommandText 发送到 Connection 并生成一个 OracleDataReader。
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            return ExecuteReader(commandText, commandType, null);
        }
        /// <summary>
        /// 将 CommandText 发送到 Connection 并生成一个 OracleDataReader。
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand command = new OracleCommand(commandText, connection);
            //如果同时传入了参数，则添加这些参数
            if (parameters != null)
            {
                foreach (OracleParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();
            //CommandBehavior.CloseConnection参数指示关闭Reader对象时关闭与其关联的Connection对象
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// 从数据库中检索单个值（例如一个聚合值）。
        /// </summary>
        /// <param name="commandText">要执行的查询Oracle文本命令</param>
        /// <returns></returns>
        public Object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// 从数据库中检索单个值（例如一个聚合值）。
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <returns></returns>
        public Object ExecuteScalar(string commandText, CommandType commandType)
        {
            return ExecuteScalar(commandText, commandType, null);
        }
        /// <summary>
        /// 从数据库中检索单个值（例如一个聚合值）。
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns></returns>
        public Object ExecuteScalar(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            object result = null;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接
                    result = command.ExecuteScalar();
                }
            }
            return result;//返回查询结果的第一行第一列，忽略其它行和列
        }
        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="commandText">要执行的查询Oracle文本命令</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, commandType, null);
        }
        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns>返回执行操作受影响的行数</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            int count = 0;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType

                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接
                    count = command.ExecuteNonQuery();
                }
            }
            return count;//返回执行增删改操作之后，数据库中受影响的行数
        }
        /// <summary>
        /// 对数据库执行增删改操作,此方法仅适合向数据增加一条记录并返回该记录的主键编号（该主键必须是自动递增的）
        /// </summary>
        /// <param name="commandText">要执行的Oracle语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者Oracle文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        /// <param name="outParameterName">要输出的参数值的参数名</param>
        /// <returns>返回整数类型的参数值</returns>
        public int ExecuteNonQueryReturnOutParameterValue(string commandText, CommandType commandType, OracleParameter[] parameters, string outParameterName)
        {
            int value = 0;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接
                    if (command.ExecuteNonQuery() > 0)
                    {
                        value = int.Parse(command.Parameters[outParameterName].Value.ToString());
                    }
                }
            }
            return value;//返回执行增删改操作之后，数据库中受影响的行数
        }
        /// <summary>
        /// 返回当前连接的数据库中所有由用户创建的数据库
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables()
        {
            DataTable data = null;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();//打开数据库连接
                data = connection.GetSchema("Tables");
            }
            return data;
        }
        /// <summary>
        /// 将List&lt;int&gt;这样的整数集合转换成(1,3,5)这样的形式,以便在Oracle语句中使用in
        /// </summary>
        /// <param name="intList">整数集合</param>
        /// <returns></returns>
        internal string ListToString(List<int> intList)
        {
            string result = " (";
            if (intList == null || intList.Count == 0)
            {
                //因为数据库中id字段都是自增的，所以不可能存在-1这样的id
                //下面的做法只是为了让执行Oracle语句时不报错
                result = result + "-1";
            }
            else
            {
                int count = intList.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    result = result + intList[i].ToString() + ",";
                }
                result = result + intList[count - 1].ToString();
            }
            result = result + ")";
            //最终将返回类似于" (1,3,4)"或者" (-1)"这样的结果
            return result;
        }

        /// <summary>
        /// 分页查询方法，适用于任何表或者视图
        /// </summary>
        /// <param name="tableName">要查询的表名或者视图名</param>
        /// <param name="where">查询的where条件</param>
        /// <param name="selectColumnName">要在in语句中查询的字段</param>
        /// <param name="orderColumnName">要排序的字段名</param>
        /// <param name="orderBy">排序方式</param>
        /// <param name="startIndex">返回记录的起始位置</param>
        /// <param name="size">返回的最大记录条数</param>
        /// <param name="parameters">查询中用到的参数集合</param>
        /// <returns>返回分页查询结果</returns>
        internal DataTable GetPagedDataTable(string tableName, string where, string selectColumnName, string orderColumnName, string orderBy, int startIndex, int size, OracleParameter[] parameters)
        {
            string orderByString = orderBy == "ASC" ? " ASC " : " DESC ";
            StringBuilder buffer = new StringBuilder(1024);
            buffer.AppendFormat("select top {0} * from {1} where {2} not in(", size, tableName, selectColumnName);
            buffer.AppendFormat("select top {0} {1} from {2} where {3} order by {4} {5}", startIndex, selectColumnName, tableName, where, orderColumnName, orderByString);
            buffer.AppendFormat(") and {0} order by {1} {2}", where, orderColumnName, orderByString);
            string commandText = buffer.ToString();
            return ExecuteDataTable(commandText, CommandType.Text, parameters);
        }

    }
}
