using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Data.SQLite;
using System.Configuration;
using SmartAutoBackup.Model;

namespace SmartAutoBackup
{
    public class DBHelperSQLite
    {
        public static string connectionString = "Data Source = " +
            ConfigurationManager.AppSettings["ConnectionString"].Replace("${base}", GlobalVar.BasePath);
        //System.AppDomain.CurrentDomain.BaseDirectory + 

        public DBHelperSQLite()
        {
            connectionString = connectionString.Replace("${base}", GlobalVar.BasePath);
        }

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>是否存在</returns>
        public static bool Exists(string SQLString)
        {
            object obj = DBHelperSQLite.GetSingle(SQLString);
            int result;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                result = 0;
            }
            else
            {
                result = int.Parse(obj.ToString());
            }
            if (result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="para">参数</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, params SQLiteParameter[] para)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        foreach (SQLiteParameter p in para)
                        {
                            cmd.Parameters.Add(p);
                        }
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="para">参数</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string SQLString, List<SQLiteParameter> para)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        foreach (SQLiteParameter p in para)
                        {
                            cmd.Parameters.Add(p);
                        }
                        int rows = 0;
                        if (SQLString.IndexOf("insert") != -1)
                            rows = Convert.ToInt32(cmd.ExecuteScalar());
                        else
                            rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public static int ExecuteSqlByTime(string SQLString, int Time)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Time;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SQLiteParameter[] para)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        foreach (SQLiteParameter par in para)
                        {
                            cmd.Parameters.Add(par);
                        }
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }


        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public static object GetSingle(string SQLString, int Times)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        return dr;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SQLiteParameter[] para)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand cmd = new SQLiteCommand(SQLString, connection);
                foreach (SQLiteParameter p in para)
                {
                    cmd.Parameters.Add(p);
                }
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                {

                    DataSet ds = new DataSet();
                    try
                    {

                        connection.Open();
                        da.Fill(ds, "ds");
                        return ds;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(SQLString, connection))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        connection.Open();
                        da.Fill(ds, "ds");
                        return ds;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public static DataSet Query(string SQLSting, int StartIndex, int PageSize)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(SQLSting, connection))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        connection.Open();
                        da.Fill(ds, StartIndex, PageSize, "ds");
                        return ds;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public static DataSet Query(string SQLString, int Times)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(SQLString, connection))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        connection.Open();
                        da.SelectCommand.CommandTimeout = Times;
                        da.Fill(ds, "ds");
                        return ds;
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
    }
}