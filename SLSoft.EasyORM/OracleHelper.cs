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
    /// ���Oracle���ݿ������ͨ����
    /// </summary>
    public class OracleDbHelper
    {
        private string connectionString;
        /// <summary>
        /// �������ݿ������ַ���
        /// </summary>
        public string ConnectionString
        {
            set { connectionString = value; }
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        public OracleDbHelper()
        {
            //#warning ע�⣬����������ַ�ʽ����ʵ����������web.config�����á�conn�������ݿ������ַ���
            //connectionString = ConfigurationManager.ConnectionStrings["OracleConnectionString"].ConnectionString;
            //connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        }
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="connectionString">���ݿ������ַ���</param>
        public OracleDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// ִ��һ����ѯ�������ؽ����
        /// </summary>
        /// <param name="commandText">Ҫִ�еĲ�ѯOracle�ı�����</param>
        /// <returns>���ز�ѯ�����</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// ִ��һ����ѯ,�����ز�ѯ���
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <returns>���ز�ѯ�����</returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType)
        {
            return ExecuteDataTable(commandText, commandType, null);
        }
        /// <summary>
        /// ִ��һ����ѯ,�����ز�ѯ���
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <param name="parameters">Transact-SQL ����洢���̵Ĳ�������</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            DataTable data = new DataTable();//ʵ����DataTable������װ�ز�ѯ�����
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//����command��CommandTypeΪָ����CommandType
                    //���ͬʱ�����˲������������Щ����
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    //ͨ��������ѯOracle��OracleCommandʵ����ʵ����OracleDataAdapter
                    OracleDataAdapter adapter = new OracleDataAdapter(command);

                    adapter.Fill(data);//���DataTable
                }
            }
            return data;
        }
        /// <summary>
        /// �� CommandText ���͵� Connection ������һ�� OracleDataAdapter��
        /// </summary>
        /// <param name="commandText">Ҫִ�еĲ�ѯOracle�ı�����</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// �� CommandText ���͵� Connection ������һ�� OracleDataReader��
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader(string commandText, CommandType commandType)
        {
            return ExecuteReader(commandText, commandType, null);
        }
        /// <summary>
        /// �� CommandText ���͵� Connection ������һ�� OracleDataReader��
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <param name="parameters">Transact-SQL ����洢���̵Ĳ�������</param>
        /// <returns></returns>
        public OracleDataReader ExecuteReader(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(connectionString);
            OracleCommand command = new OracleCommand(commandText, connection);
            //���ͬʱ�����˲������������Щ����
            if (parameters != null)
            {
                foreach (OracleParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();
            //CommandBehavior.CloseConnection����ָʾ�ر�Reader����ʱ�ر����������Connection����
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
        /// <summary>
        /// �����ݿ��м�������ֵ������һ���ۺ�ֵ����
        /// </summary>
        /// <param name="commandText">Ҫִ�еĲ�ѯOracle�ı�����</param>
        /// <returns></returns>
        public Object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// �����ݿ��м�������ֵ������һ���ۺ�ֵ����
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <returns></returns>
        public Object ExecuteScalar(string commandText, CommandType commandType)
        {
            return ExecuteScalar(commandText, commandType, null);
        }
        /// <summary>
        /// �����ݿ��м�������ֵ������һ���ۺ�ֵ����
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <param name="parameters">Transact-SQL ����洢���̵Ĳ�������</param>
        /// <returns></returns>
        public Object ExecuteScalar(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            object result = null;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//����command��CommandTypeΪָ����CommandType
                    //���ͬʱ�����˲������������Щ����
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//�����ݿ�����
                    result = command.ExecuteScalar();
                }
            }
            return result;//���ز�ѯ����ĵ�һ�е�һ�У����������к���
        }
        /// <summary>
        /// �����ݿ�ִ����ɾ�Ĳ���
        /// </summary>
        /// <param name="commandText">Ҫִ�еĲ�ѯOracle�ı�����</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, null);
        }
        /// <summary>
        /// �����ݿ�ִ����ɾ�Ĳ���
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType)
        {
            return ExecuteNonQuery(commandText, commandType, null);
        }
        /// <summary>
        /// �����ݿ�ִ����ɾ�Ĳ���
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <param name="parameters">Transact-SQL ����洢���̵Ĳ�������</param>
        /// <returns>����ִ�в�����Ӱ�������</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType, OracleParameter[] parameters)
        {
            int count = 0;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//����command��CommandTypeΪָ����CommandType

                    //���ͬʱ�����˲������������Щ����
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//�����ݿ�����
                    count = command.ExecuteNonQuery();
                }
            }
            return count;//����ִ����ɾ�Ĳ���֮�����ݿ�����Ӱ�������
        }
        /// <summary>
        /// �����ݿ�ִ����ɾ�Ĳ���,�˷������ʺ�����������һ����¼�����ظü�¼��������ţ��������������Զ������ģ�
        /// </summary>
        /// <param name="commandText">Ҫִ�е�Oracle���</param>
        /// <param name="commandType">Ҫִ�еĲ�ѯ�������ͣ���洢���̻���Oracle�ı�����</param>
        /// <param name="parameters">Transact-SQL ����洢���̵Ĳ�������</param>
        /// <param name="outParameterName">Ҫ����Ĳ���ֵ�Ĳ�����</param>
        /// <returns>�����������͵Ĳ���ֵ</returns>
        public int ExecuteNonQueryReturnOutParameterValue(string commandText, CommandType commandType, OracleParameter[] parameters, string outParameterName)
        {
            int value = 0;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(commandText, connection))
                {
                    command.CommandType = commandType;//����command��CommandTypeΪָ����CommandType
                    //���ͬʱ�����˲������������Щ����
                    if (parameters != null)
                    {
                        foreach (OracleParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//�����ݿ�����
                    if (command.ExecuteNonQuery() > 0)
                    {
                        value = int.Parse(command.Parameters[outParameterName].Value.ToString());
                    }
                }
            }
            return value;//����ִ����ɾ�Ĳ���֮�����ݿ�����Ӱ�������
        }
        /// <summary>
        /// ���ص�ǰ���ӵ����ݿ����������û����������ݿ�
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables()
        {
            DataTable data = null;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();//�����ݿ�����
                data = connection.GetSchema("Tables");
            }
            return data;
        }
        /// <summary>
        /// ��List&lt;int&gt;��������������ת����(1,3,5)��������ʽ,�Ա���Oracle�����ʹ��in
        /// </summary>
        /// <param name="intList">��������</param>
        /// <returns></returns>
        internal string ListToString(List<int> intList)
        {
            string result = " (";
            if (intList == null || intList.Count == 0)
            {
                //��Ϊ���ݿ���id�ֶζ��������ģ����Բ����ܴ���-1������id
                //���������ֻ��Ϊ����ִ��Oracle���ʱ������
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
            //���ս�����������" (1,3,4)"����" (-1)"�����Ľ��
            return result;
        }

        /// <summary>
        /// ��ҳ��ѯ�������������κα������ͼ
        /// </summary>
        /// <param name="tableName">Ҫ��ѯ�ı���������ͼ��</param>
        /// <param name="where">��ѯ��where����</param>
        /// <param name="selectColumnName">Ҫ��in����в�ѯ���ֶ�</param>
        /// <param name="orderColumnName">Ҫ������ֶ���</param>
        /// <param name="orderBy">����ʽ</param>
        /// <param name="startIndex">���ؼ�¼����ʼλ��</param>
        /// <param name="size">���ص�����¼����</param>
        /// <param name="parameters">��ѯ���õ��Ĳ�������</param>
        /// <returns>���ط�ҳ��ѯ���</returns>
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
