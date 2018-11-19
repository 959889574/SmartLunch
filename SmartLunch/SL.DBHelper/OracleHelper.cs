using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using Oracle.ManagedDataAccess.Client;

namespace SL.DBHelper
{
    /// <summary>
    /// 名称: OracleLHelper
    /// 功能: Oracle数据库访问类
    /// 作者: 杨贵柱
    /// 版本: V1.0
    /// 时间: 2017-1-22
    /// </summary>
    public class OracleHelper
    {
        /// <summary>
        /// 默认数据库连接字符串
        /// </summary>		
        public  string ConnectionString =ConfigurationManager.ConnectionStrings["DefaultConnString"].ConnectionString;

        public OracleHelper(string connectStr = null)
        {
            if (!string.IsNullOrEmpty(connectStr))
            {
                ConnectionString = connectStr;
            }
        }

        public OracleTransaction CurrentTransaction;

        #region  Prepare a command for execution
        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">OracleCommand object</param>
        /// <param name="conn">OracleConnection object</param>
        /// <param name="trans">OracleTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">DbParameters to use in the command</param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, DbParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {

                //   update by :yang gui zhu 以下语句添加参数时，报错：OracleParameter object is already contained in a collection ,使用以上AddRange解决。
//                foreach (DbParameter parm in cmdParms)
//                {
//                    if (parm.Value == null || ((parm.DbType == DbType.Date || parm.DbType == DbType.DateTime || parm.DbType == DbType.DateTime2) && DateTime.Parse(parm.Value.ToString()) == DateTime.MinValue))
//                    {
//                        parm.Value = DBNull.Value;
//                    }
//                    cmd.Parameters.Add(parm);
//                }
                cmd.Parameters.AddRange(cmdParms);
            }
        }
        #endregion

        #region 执行一条T_SQL命令返回影响的行数的方法(适合执行Update、Delete、Insert三种T_SQL语句)
        /// <summary>
        /// 执行一条T_SQL命令返回影响的行数的方法(适合执行Update、Delete、Insert三种T_SQL语句)
        /// </summary>
        /// <param name="CommandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回影响的行数(row)</returns>
        public  int ExecuteNonQuery(string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            if (type == CommandType.Text)
            {
                cmd.BindByName = true;    
            }
            
            if (CurrentTransaction != null)
            {
                PrepareCommand(cmd, CurrentTransaction.Connection, null, type, commandText, paraList);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
            else
            {
                try
                {
                    //using (OracleConnection conn = new OracleConnection(ConnectionString))
                    using (OracleConnection conn = new OracleConnection(ConnectionString))
                    {
                        PrepareCommand(cmd, conn, null, type, commandText, paraList);
                        int val = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return val;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public  int ExecuteNonQueryWithParameterObject(string commandText, CommandType type, object parameterObj)
        {
            return ExecuteNonQuery(commandText, type, GetParameters(commandText, parameterObj));
        }

        private  DbParameter[] GetParameters(string commandText, object parameterObj)
        {
            if (parameterObj == null)
            {
                return null;
            }
            else
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                Type type = parameterObj.GetType();
                if (type.Name == "ExpandoObject")
                {
                    ExpandoObject dynamicObj = (ExpandoObject)parameterObj;
                    dynamicObj.ToList().ForEach(x =>
                          {
                              if (commandText.ToLower().Contains(":" + x.Key.ToLower()))
                              {
                                  parameters.Add(new OracleParameter(":" + x.Key, x.Value));
                              }
                          });
                }
                else
                {
                    type.GetProperties().ToList().ForEach(x =>
                         {
                             if (commandText.ToLower().Contains(":" + x.Name.ToLower()))
                             {
                                 parameters.Add(new OracleParameter(":" + x.Name, x.GetValue(parameterObj, null)));
                             }
                         });
                }
                return parameters.ToArray();
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回影响的行数的方法(适合执行Update、Delete、Insert三种T_SQL语句)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="CommandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回影响的行数(row)</returns>
        public  int ExecuteNonQuery(string connectionString, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, type, commandText, paraList);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public int ExecuteNonQuery(IDbConnection connection, string commandText, CommandType type, DbParameter[] paraList)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行一条T_SQL命令返回影响的行数的方法(适合执行Update、Delete、Insert三种T_SQL语句)
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="CommandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回影响的行数(row)</returns>
        public  int ExecuteNonQuery(OracleConnection connection, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, type, commandText, paraList);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行一条T_SQL命令返回影响的行数的方法(适合执行Update、Delete、Insert三种T_SQL语句)
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="CommandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回影响的行数(row)</returns>
        public  int ExecuteNonQuery(OracleTransaction trans, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, type, commandText, paraList);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion

        #region 执行一条T_SQL命令返回首行首列的方法(适合执行Select命令中含有聚合函数的T_SQL语句)
        /// <summary>
        /// 执行一条T_SQL命令返回首行首列的方法(适合执行Select命令中含有聚合函数的T_SQL语句)
        /// </summary>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回首行首列(obj)</returns>
        public  object ExecuteScalar(string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            if (CurrentTransaction != null)
            {
                PrepareCommand(cmd, CurrentTransaction.Connection, null, type, commandText, paraList);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
            else
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    PrepareCommand(cmd, connection, null, type, commandText, paraList);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return val;
                }
            }
        }

        public  object ExecuteScalarWithParameterObject(string commandText, CommandType type, object parameterObj)
        {
            return ExecuteScalar(commandText, type, GetParameters(commandText, parameterObj));
        }

        /// <summary>
        /// 执行一条T_SQL命令返回首行首列的方法(适合执行Select命令中含有聚合函数的T_SQL语句)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回首行首列(obj)</returns>
        public  object ExecuteScalar(string connectionString, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, type, commandText, paraList);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回首行首列的方法(适合执行Select命令中含有聚合函数的T_SQL语句)
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回首行首列(obj)</returns>
        public  object ExecuteScalar(OracleConnection connection, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            PrepareCommand(cmd, connection, null, type, commandText, paraList);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行一条T_SQL命令返回首行首列的方法(适合执行Select命令中含有聚合函数的T_SQL语句)
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回首行首列(obj)</returns>
        public  object ExecuteScalar(OracleTransaction trans, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            PrepareCommand(cmd, trans.Connection, trans, type, commandText, paraList);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        #endregion

        #region 执行一条T_SQL命令返回IDataReader的方法(适合连接模式执行Select命令的T_SQL语句)
        /// <summary>
        /// 执行一条T_SQL命令返回OracleDataReader的方法(适合连接模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据读取器(reader)</returns>
        public  OracleDataReader ExecuteReader(string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            if (CurrentTransaction != null)
            {
                PrepareCommand(cmd, CurrentTransaction.Connection, null, type, commandText, paraList);
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return rdr;
            }
            else
            {
                OracleConnection conn = new OracleConnection(ConnectionString);

                // we use a try/catch here because if the method throws an exception we want to 
                // close the connection throw code, because no datareader will exist, hence the 
                // commandBehaviour.CloseConnection will not work
                try
                {
                    PrepareCommand(cmd, conn, null, type, commandText, paraList);
                    OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Parameters.Clear();
                    return rdr;
                }
                catch
                {
                    conn.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回OracleDataReader的方法(适合连接模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据读取器(reader)</returns>
        public  OracleDataReader ExecuteReader(string connectionString, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, type, commandText, paraList);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回OracleDataReader的方法(适合连接模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据读取器(reader)</returns>
        public  OracleDataReader ExecuteReader(OracleConnection connection, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, connection, null, type, commandText, paraList);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回OracleDataReader的方法(适合连接模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据读取器(reader)</returns>
        public  OracleDataReader ExecuteReader(OracleTransaction trans, string commandText, CommandType type, DbParameter[] paraList)
        {
            OracleCommand cmd = new OracleCommand();
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, trans.Connection, trans, type, commandText, paraList);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                trans.Connection.Close();
                throw;
            }
        }
        #endregion

        #region 执行一条T_SQL命令返回DataSet的方法(适合断开模式执行Select命令的T_SQL语句)
        /// <summary>
        /// 执行一条T_SQL命令返回DataSet的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(ds)</returns> 
        public  DataSet ExecuteDataSet(string commandText, CommandType type, params DbParameter[] paraList)
        {
            DataSet data = new DataSet();
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            if (CurrentTransaction != null)
            {
                PrepareCommand(cmd, CurrentTransaction.Connection, null, type, commandText, paraList);
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(data);
                cmd.Parameters.Clear();

                return data;
            }
            else
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    PrepareCommand(cmd, connection, null, type, commandText, paraList);
                    OracleDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(data);
                    cmd.Parameters.Clear();

                    return data;
                }
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回DataSet的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(ds)</returns> 
        public  DataSet ExecuteDataSet(string connectionString, string commandText, CommandType type, DbParameter[] paraList)
        {
            DataSet data = new DataSet();
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, type, commandText, paraList);
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(data);
                return data;
            }
        }

        /// <summary>
        /// 执行一条T_SQL命令返回DataSet的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(ds)</returns> 
        public  DataSet ExecuteDataSet(OracleConnection connection, string commandText, CommandType type, DbParameter[] paraList)
        {
            DataSet data = new DataSet();
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            PrepareCommand(cmd, connection, null, type, commandText, paraList);
            OracleDataAdapter adapter = new OracleDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(data);
            return data;
        }

        /// <summary>
        /// 执行一条T_SQL命令返回DataSet的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(ds)</returns> 
        public  DataSet ExecuteDataSet(OracleTransaction trans, string commandText, CommandType type, DbParameter[] paraList)
        {
            DataSet data = new DataSet();
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            PrepareCommand(cmd, trans.Connection, trans, type, commandText, paraList);
            OracleDataAdapter adapter = new OracleDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(data);
            return data;
        }
        #endregion

        #region 执行一条T_SQL命令返回DataTable的方法(适合断开模式执行Select命令的T_SQL语句)
        /// <summary>
        /// 执行一条T_SQL命令返回DataTable的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(dt)</returns> 
        public  DataTable ExecuteDataTable(string commandText, CommandType type, DbParameter[] paraList)
        {
            DataTable data = new DataTable();
            OracleCommand cmd = new OracleCommand();
            if (CurrentTransaction != null)
            {
                cmd.BindByName = true;
                PrepareCommand(cmd, CurrentTransaction.Connection, null, type, commandText, paraList);
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(data);
                return data;
            }
            else
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    cmd.BindByName = true;
                    PrepareCommand(cmd, connection, null, type, commandText, paraList);
                    OracleDataAdapter adapter = new OracleDataAdapter();
                    adapter.SelectCommand = cmd;
                    adapter.Fill(data);
                    return data;
                }
            }
        }
        public  DataTable ExecuteDataTableWithParameterObject(string commandText, CommandType type, object parameterObj)
        {
            return ExecuteDataTable(commandText, type, GetParameters(commandText, parameterObj));
        }
        /// <summary>
        /// 执行一条T_SQL命令返回DataTable的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(dt)</returns> 
        public  DataTable ExecuteDataTable(string connectionString, string commandText, CommandType type, DbParameter[] paraList)
        {
            DataTable data = new DataTable();
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                cmd.BindByName = true;
                PrepareCommand(cmd, connection, null, type, commandText, paraList);
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(data);
                return data;
            }
        }
        /// <summary>
        /// 执行一条T_SQL命令返回DataTable的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="connection">数据库连接对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(dt)</returns> 
        public  DataTable ExecuteDataTable(OracleConnection connection, string commandText, CommandType type, DbParameter[] paraList)
        {
            DataTable data = new DataTable();
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            PrepareCommand(cmd, connection, null, type, commandText, paraList);
            OracleDataAdapter adapter = new OracleDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(data);
            return data;
        }
        /// <summary>
        /// 执行一条T_SQL命令返回DataTable的方法(适合断开模式执行Select命令的T_SQL语句)
        /// </summary>
        /// <param name="trans">事务对象</param>
        /// <param name="commandText">T_SQL命令字符串或存储过程名</param>
        /// <param name="type">T_SQL命令的类型(文本类型:Text或存储过程类型:StoredProcedure)</param>
        /// <param name="paraList">T_SQL命令的参数集合(无参数添加时即为null)</param>
        /// <returns>返回数据记录集(dt)</returns> 
        public  DataTable ExecuteDataTable(OracleTransaction trans, string commandText, CommandType type, DbParameter[] paraList)
        {
            DataTable data = new DataTable();
            OracleCommand cmd = new OracleCommand();
            cmd.BindByName = true;
            PrepareCommand(cmd, trans.Connection, trans, type, commandText, paraList);
            OracleDataAdapter adapter = new OracleDataAdapter();
            adapter.SelectCommand = cmd;
            adapter.Fill(data);
            return data;
        }

        public  bool ExecuteSqlNonQueryTrans(string[] strSQL, IList<IList<OracleParameter>> paramLists)
        {
            OracleConnection conn = new OracleConnection(ConnectionString);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            OracleTransaction tran = conn.BeginTransaction();
            OracleCommand cmd = new OracleCommand();
            cmd.Transaction = tran;
            cmd.Connection = conn;
            try
            {
                for (int i = 0; i < strSQL.Length; i++)
                {
                    cmd.CommandText = strSQL[i];
                    cmd.BindByName = true;
                    if (null != paramLists && paramLists[i] != null)
                    {
                        cmd = AddCommandParam(cmd, paramLists[i]);
                    }
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                tran.Commit();
                return true;
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw e;
            }
            finally
            {
                conn.Close();
            }
        }

        protected  OracleCommand AddCommandParam(OracleCommand com, IList<OracleParameter> paramList)
        {
            if (com == null)
                return null;
            com.Parameters.Clear();
            if (paramList != null && paramList.Count > 0)
            {
                com.Parameters.AddRange(paramList.ToArray());
            }
            return com;
        }

        #endregion

        #region 获取OracleTransaction
        /// <summary>
        /// 获取OracleTransaction
        /// </summary>
        /// <returns></returns>
        public  OracleTransaction BeginTransaction()
        {
            OracleConnection conn = new OracleConnection(ConnectionString);
            conn.Open();
            return conn.BeginTransaction();
        }
        #endregion

        /// <summary>  
        /// 批量插入数据  
        /// </summary>  
        /// <param name="recordCount">记录数</param>  
        /// <param name="sql">sql</param>  
        /// <param name="param">参数</param>  
        public  void ExecuteSqlBat(int recordCount, string sql, params OracleParameter[] param)
        {
            OracleConnection conn = new OracleConnection(ConnectionString);
            OracleCommand command = new OracleCommand { Connection = conn };
            conn.Open();
            var tx = CurrentTransaction ?? conn.BeginTransaction();
            try
            {
                command.Transaction = tx;
                //这个参数需要指定每次批插入的记录数  
                command.ArrayBindCount = recordCount;
                //用到的是数组,而不是单个的值,这就是它独特的地方  
                command.CommandText = sql;
                command.BindByName = true;
                foreach (OracleParameter t in param)
                {
                    command.Parameters.Add(t);
                }

                //这个调用将把参数数组传进SQL,同时写入数据库  
                command.ExecuteNonQuery();
                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        public  string ExecuteProcudure(string argProcName, IList<OracleParameter> paraList)
        {

            OracleConnection conn = new OracleConnection(ConnectionString);
            try
            {
                OracleCommand cmd = new OracleCommand();
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = argProcName;
                cmd = AddCommandParam(cmd, paraList);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();

                return "OK";
            }
            catch (Exception ex)
            {
                return "执行异常:" + ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

    }
}
