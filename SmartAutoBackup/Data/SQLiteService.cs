using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Web;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartAutoBackup
{
    public class SQLiteClass<T>
        where T : class,new()
    {
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] ", typeof(T).UnderlyingSystemType.Name);
            return Query(sql, null);
        }

        /// <summary>
        /// 根据条件查询N条数据
        /// </summary>
        /// <param name="where">条件,可追加排序</param>
        /// <param name="n">多少条数据</param>
        /// <param name="ps">参数</param>
        /// <returns>返回T的集合</returns>
        public List<T> FindAll(string where, int n, params SQLiteParameter[] ps)
        {
            return FindAll(where + " limit " + n, ps);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="ps">参数</param>
        /// <returns></returns>
        public List<T> FindAll(string where, params SQLiteParameter[] ps)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] {1} ", typeof(T).UnderlyingSystemType.Name, where);
            return Query(sql, ps);
        }

        /// <summary>
        /// 根据字段名查询
        /// </summary>
        /// <param name="ColName">字段名</param>
        /// <param name="ColValue">字段值</param>
        /// <returns></returns>
        public List<T> FindAllByColName(string ColName, object ColValue)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] where [{1}] =@v ", typeof(T).UnderlyingSystemType.Name, ColName);
            return Query(sql, new SQLiteParameter("@v", ColValue));
        }

        /// <summary>
        /// 根据字段名查询N条数据
        /// </summary>
        /// <param name="ColName">字段名</param>
        /// <param name="ColValue">字段值</param>
        /// <param name="n">N条数据</param>
        /// <returns></returns>
        public List<T> FindAllByColName(string ColName, object ColValue, int n)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] where [{1}] =@v limit {2}", typeof(T).UnderlyingSystemType.Name, ColName, n);
            return Query(sql, new SQLiteParameter("@v", ColValue));
        }

        /// <summary>
        /// 带条件的分页查询
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="index">当前页码</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="ps">参数</param>
        /// <returns></returns>
        public List<T> FindAll(string where, int index, int pageSize, params SQLiteParameter[] ps)
        {
            if (pageSize < 0)
                pageSize = 0;
            if (index < 0)
                index = 0;
            if (index > 0)
                index -= 1;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] {1}  limit @offset,@size",
                typeof(T).UnderlyingSystemType.Name, where, index * pageSize, pageSize);
            SQLiteParameter[] pars = new SQLiteParameter[] 
        {
            new SQLiteParameter("@offset",index * pageSize),
            new SQLiteParameter("@size",pageSize)
        };
            List<SQLiteParameter> lists = new List<SQLiteParameter>();
            foreach (var i in pars)
            {
                lists.Add(i);
            }
            foreach (var j in ps)
            {
                lists.Add(j);
            }
            SQLiteParameter[] pp = lists.ToArray();
            return Query(sql, pp);
        }

        public List<T> FindAll(int n)
        {
            return FindAll(0, n);
        }

        public List<T> FindAll(int index, int pageSize)
        {
            if (pageSize < 0)
                pageSize = 0;
            if (index > 0)
                index = index - 1;
            if (index < 0)
                index = 0;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] order by id desc limit @offset,@size",
                typeof(T).UnderlyingSystemType.Name);
            SQLiteParameter[] pars = new SQLiteParameter[] 
        {
            new SQLiteParameter("@offset",index * pageSize),
            new SQLiteParameter("@size",pageSize)
        };
            return Query(sql, pars);
        }

        public List<T> FindAll(int index, int pageSize, string orderby, OrderBy order)
        {
            if (pageSize < 0)
                pageSize = 0;
            if (index < 0)
                index = 0;
            if (index > 0)
                index -= 1;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] order by [{1}] {2} limit @offset,@size",
                typeof(T).UnderlyingSystemType.Name, orderby, order.ToString());
            SQLiteParameter[] pars = new SQLiteParameter[] 
        {
            new SQLiteParameter("@offset",index * pageSize),
            new SQLiteParameter("@size",pageSize)
        };
            return Query(sql, pars);
        }


        public List<T> Query(StringBuilder sql)
        {
            return Query(sql.ToString(), null);
        }

        public List<T> Query(string sql)
        {
            return Query(sql, null);
        }

        public List<T> Query(StringBuilder sql, params SQLiteParameter[] pars)
        {
            return Query(sql.ToString(), pars);
        }

        public List<T> Query(string sql, params SQLiteParameter[] pars)
        {
            DataSet ds;
            if (pars == null)
                ds = DBHelperSQLite.Query(sql);
            else
                ds = DBHelperSQLite.Query(sql, pars);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<T> list = new List<T>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    T t = new T();
                    foreach (var item in t.GetType().GetProperties())
                    {
                        var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                        if (propertyarrlist.Length != 1)
                            continue;
                        var xx = dr[item.Name];
                        if (xx.GetType().Name.ToLower() != "dbnull")
                            item.SetValue(t, xx, null);
                        else
                            item.SetValue(t, GetValue(item), null);
                    }
                    list.Add(t);
                }
                return list;
            }
            return new List<T>();
        }

        public List<TT> Query<TT>(string sql, params SQLiteParameter[] pars)
            where TT : class,new()
        {
            DataSet ds;
            if (pars == null)
                ds = DBHelperSQLite.Query(sql);
            else
                ds = DBHelperSQLite.Query(sql, pars);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                List<TT> list = new List<TT>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    TT t = new TT();
                    foreach (var item in t.GetType().GetProperties())
                    {
                        var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                        if (propertyarrlist.Length != 1)
                            continue;
                        var xx = dr[item.Name];
                        if (xx.GetType().Name.ToLower() != "dbnull")
                            item.SetValue(t, xx, null);
                        else
                            item.SetValue(t, GetValue(item), null);
                    }
                    list.Add(t);
                }
                return list;
            }
            return new List<TT>();
        }

        public int Add(T t)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder csql = new StringBuilder();
            StringBuilder vsql = new StringBuilder();
            var ps = t.GetType().GetProperties();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            for (int i = 0; i < ps.Length; i++)
            {
                var item = ps[i];
                var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                if (propertyarrlist.Length != 1)
                    continue;
                var itemValue = item.GetValue(t, null);
                if (itemValue == null)
                {
                    itemValue = GetValue(item);
                }
                if (item.Name.ToLower() != "id")
                {
                    csql.AppendFormat("[{0}],", item.Name);
                    vsql.AppendFormat("@{0},", item.Name);
                    parameters.Add(new SQLiteParameter("@" + item.Name, itemValue));
                }
            }
            sql.AppendFormat("insert into [{0}]({1}) values({2})",
                t.GetType().UnderlyingSystemType.Name,
                csql.ToString().TrimEnd(','),
                vsql.ToString().TrimEnd(','));

            return DBHelperSQLite.ExecuteSql(sql.ToString() + ";select last_insert_rowid();", parameters);
            //return Convert.ToInt32(DBHelperSQLite.GetSingle("select last_insert_rowid() newid"));
        }

        public int Edit(T t)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder csql = new StringBuilder();
            var ps = t.GetType().GetProperties();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            for (int i = 0; i < ps.Length; i++)
            {
                var item = ps[i];
                var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                if (propertyarrlist.Length != 1)
                    continue;
                var itemValue = item.GetValue(t, null);
                if (itemValue == null)
                {
                    itemValue = GetValue(item);
                }
                if (item.Name.ToLower() != "id")
                {
                    var _value = itemValue;
                    csql.AppendFormat("[{0}] = @{0},", item.Name);
                    parameters.Add(new SQLiteParameter("@" + item.Name, _value));
                }
            }
            sql.AppendFormat("update [{0}] set {1} where {2} = @{2}",
                t.GetType().UnderlyingSystemType.Name,
                csql.ToString().TrimEnd(','),
                "Id");
            parameters.Add(new SQLiteParameter("@Id", t.GetType().GetProperty("Id").GetValue(t, null)));
            return DBHelperSQLite.ExecuteSql(sql.ToString(), parameters);
        }


        public int EditByWhere(string where, params SQLiteParameter[] pars)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder csql = new StringBuilder();
            T t = new T();
            var ps = t.GetType().GetProperties();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            for (int i = 0; i < ps.Length; i++)
            {
                var item = ps[i];
                var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                if (propertyarrlist.Length != 1)
                    continue;
                if (item.Name.ToLower() != "id")
                {
                    var _value = item.GetValue(t, null);
                    if (i < ps.Length - 1)
                    {
                        csql.AppendFormat("[{0}] = @{0},", item.Name);
                    }
                    else
                    {
                        csql.AppendFormat("[{0}] = @{0}", item.Name);
                    }
                    parameters.Add(new SQLiteParameter("@" + item.Name, _value));
                }
            }
            sql.AppendFormat("update [{0}] set {1} {2}",
                t.GetType().UnderlyingSystemType.Name,
                csql.ToString(),
                where);
            foreach (var item in pars)
            {
                parameters.Add(item);
            }
            return DBHelperSQLite.ExecuteSql(sql.ToString(), parameters);
        }

        public T GetByColName(string name, object value)
        {
            string sql = string.Format("select * from [{0}] where [{1}] = @value", typeof(T).UnderlyingSystemType.Name, name);
            SQLiteParameter p = new SQLiteParameter("@value", value);
            DataSet ds = DBHelperSQLite.Query(sql.ToString(), p);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                T t = new T();
                foreach (var item in t.GetType().GetProperties())
                {
                    var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                    if (propertyarrlist.Length != 1)
                        continue;
                    var xx = ds.Tables[0].Rows[0][item.Name];
                    if (xx.GetType().Name.ToLower() != "dbnull")
                        item.SetValue(t, xx, null);
                    else
                        item.SetValue(t, GetValue(item), null);
                }
                return t;
            }
            return null;
        }

        public T GetById(long id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] where id = @id", typeof(T).UnderlyingSystemType.Name);
            SQLiteParameter p = new SQLiteParameter("@id", id);
            DataSet ds = DBHelperSQLite.Query(sql.ToString(), p);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                T t = new T();
                foreach (var item in t.GetType().GetProperties())
                {
                    var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                    if (propertyarrlist.Length != 1)
                        continue;
                    var xx = ds.Tables[0].Rows[0][item.Name];
                    if (xx.GetType().Name.ToLower() != "dbnull")
                        item.SetValue(t, xx, null);
                    else
                        item.SetValue(t, GetValue(item), null);
                }
                return t;
            }
            return null;
        }

        public T GetByWhere(string where, params SQLiteParameter[] ps)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from [{0}] {1}", typeof(T).UnderlyingSystemType.Name, where);
            DataSet ds;
            if (ps != null)
            {
                ds = DBHelperSQLite.Query(sql.ToString(), ps);
            }
            else
            {
                ds = DBHelperSQLite.Query(sql.ToString());
            }
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                T t = new T();
                foreach (var item in t.GetType().GetProperties())
                {
                    var propertyarrlist = item.GetCustomAttributes(typeof(DataField), false);
                    if (propertyarrlist.Length != 1)
                        continue;
                    var xx = ds.Tables[0].Rows[0][item.Name];
                    if (xx.GetType().Name.ToLower() != "dbnull")
                        item.SetValue(t, xx, null);
                    else
                        item.SetValue(t, GetValue(item), null);
                }
                return t;
            }
            return null;
        }

        public int Delete(long id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from [{0}] where id=@id", typeof(T).UnderlyingSystemType.Name);
            SQLiteParameter p = new SQLiteParameter("@id", id);
            return DBHelperSQLite.ExecuteSql(sql.ToString(), p);
        }

        public int Delete(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from [{0}] where id=@id", typeof(T).UnderlyingSystemType.Name);
            SQLiteParameter p = new SQLiteParameter("@id", id);
            return DBHelperSQLite.ExecuteSql(sql.ToString(), p);
        }

        public int GetCount()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select count(*) from [{0}]", typeof(T).UnderlyingSystemType.Name);
            return Convert.ToInt32(DBHelperSQLite.GetSingle(sql.ToString()));
        }

        public int GetCount(string where)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select count(*) from [{0}] {1}", typeof(T).UnderlyingSystemType.Name, where);
            return Convert.ToInt32(DBHelperSQLite.GetSingle(sql.ToString()));
        }

        public int GetCount(string where, params SQLiteParameter[] ps)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select count(*) from [{0}] {1}", typeof(T).UnderlyingSystemType.Name, where);
            return Convert.ToInt32(DBHelperSQLite.GetSingle(sql.ToString(), ps));
        }

        public int DeleteWhere(string where, params SQLiteParameter[] p)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("delete from [{0}] {1}", typeof(T).UnderlyingSystemType.Name, where);
            if (p != null)
            {
                return DBHelperSQLite.ExecuteSql(sql.ToString(), p);
            }
            else
            {
                return DBHelperSQLite.ExecuteSql(sql.ToString());
            }
        }

        public TT GetSingle<TT>(string sql)
            where TT : IComparable
        {
            object obj = DBHelperSQLite.GetSingle(sql);
            return (TT)Convert.ChangeType(obj, typeof(TT));
        }

        public TT GetSingle<TT>(string sql, params SQLiteParameter[] p)
            where TT : IComparable
        {
            object obj = DBHelperSQLite.GetSingle(sql, p);
            if (obj == null)
                obj = 0;
            return (TT)Convert.ChangeType(obj, typeof(TT));
        }

        private object GetValue(PropertyInfo item)
        {
            var name = item.PropertyType.FullName;
            object val = new object();
            switch (name)
            {
                case "System.Int64":
                    val = 0;
                    break;
                case "System.Int32":
                    val = 0;
                    break;
                case "System.String":
                    val = string.Empty;
                    break;
                case "System.DateTime":
                    val = new DateTime();
                    break;
                case "System.Boolean":
                    val = false;
                    break;
                case "System.Decimal":
                    val = 0m;
                    break;
                default:
                    val = null;
                    break;
            }
            return val;
        }

    }

    public enum OrderBy
    {
        Desc,
        Asc
    }

    /// <summary>
    /// 有该标记的属性为数据库中对应的字段
    /// </summary>
    public class DataField : Attribute
    {
        //I don't know do what.....
    }

    public class AdinetClass
    {
        public static AdinetDataRow Single(string sql, params SQLiteParameter[] pars)
        {
            DataSet ds;
            if (pars == null)
                ds = DBHelperSQLite.Query(sql);
            else
                ds = DBHelperSQLite.Query(sql, pars);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return new AdinetDataRow(ds.Tables[0].Rows[0]);
            }
            return null;
        }

        public static int Execute(string sql, params SQLiteParameter[] pars)
        {
            int ds;
            if (pars == null)
                ds = DBHelperSQLite.ExecuteSql(sql);
            else
                ds = DBHelperSQLite.ExecuteSql(sql, pars);
            return ds;
        }

        public static List<AdinetDataRow> CommentDataRow(string sql, params SQLiteParameter[] pars)
        {
            List<AdinetDataRow> list = new List<AdinetDataRow>();
            DataSet ds;
            if (pars == null)
                ds = DBHelperSQLite.Query(sql);
            else
                ds = DBHelperSQLite.Query(sql, pars);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new AdinetDataRow(dr));
                }
            }
            return list;
        }

        public static List<AdinetDataRow> CommentDataRow(string sql,  int index, int pageSize, params SQLiteParameter[] pars)
        {
            if (pageSize < 0)
                pageSize = 0;
            if (index < 0)
                index = 0;
            if (index > 0)
                index -= 1;
            sql = sql.ToLower().Replace("select", "");
            string _sql = string.Format("select {0} limit @offset,@pagesize", sql).Replace("*_", "Adinet_");
            List<SQLiteParameter> pl = new List<SQLiteParameter>();
            pl.Add(new SQLiteParameter("@offset", index * pageSize));
            pl.Add(new SQLiteParameter("@pagesize", pageSize));
            if (pars != null)
                pl.AddRange(pars);
            return CommentDataRow(_sql, pl.ToArray());
        }
        /*
        public int Add(Hashtable tables)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder csql = new StringBuilder();
            StringBuilder vsql = new StringBuilder();
            List<SQLiteParameter> parameters = new List<SQLiteParameter>();
            for (int i = 0; i < tables.Count; i++)
            {
                var item = tables[i];
                
                if (item.ToLower() != "id")
                {
                    csql.AppendFormat("[{0}],", item.Name);
                    vsql.AppendFormat("@{0},", item.Name);
                    parameters.Add(new SQLiteParameter("@" + item.Name, itemValue));
                }
            }
            sql.AppendFormat("insert into [{0}]({1}) values({2})",
                t.GetType().UnderlyingSystemType.Name,
                csql.ToString().TrimEnd(','),
                vsql.ToString().TrimEnd(','));

            return DBHelperSQLite.ExecuteSql(sql.ToString() + ";select last_insert_rowid();", parameters);
            //return Convert.ToInt32(DBHelperSQLite.GetSingle("select last_insert_rowid() newid"));
        }*/
    }

    public class AdinetDataRow
    {

        protected System.Data.DataRow _row;
        public AdinetDataRow(DataRow dr)
        {
            _row = dr;
        }
        public object this[int columnIndex]
        {
            get
            {
                if (columnIndex >= _row.Table.Columns.Count)
                {
                    return "索引错误";
                }
                else
                {
                    return _row[columnIndex];
                }
            }
        }
        public object this[string columnName]
        {
            get
            {
                if (!_row.Table.Columns.Contains(columnName))
                {
                    return "字段错误";
                }
                else
                {
                    return _row[columnName];
                }
            }
        }
        public bool IsNull(int columnIndex)
        {
            return _row.IsNull(columnIndex);
        }

        public bool IsNull(string columnName)
        {
            return _row.IsNull(columnName);
        }
    }
}

//public class EntityContext<T>
//{
//    private static readonly MethodInfo getValueMethod = typeof(IDataRecord).GetMethod("get_Item", new Type[] { typeof(int) });
//    private static readonly MethodInfo isDBNullMethod = typeof(IDataRecord).GetMethod("IsDBNull", new Type[] { typeof(int) });
//    private delegate T Load(IDataRecord dataRecord);
//    private Load handler;

//    private EntityContext() { }

//    public T Build(IDataRecord dataRecord)
//    {
//        return handler(dataRecord);
//    }

//    public static EntityContext<T> CreateBuilder(IDataRecord dataRecord)
//    {
//        EntityContext<T> dynamicBuilder = new EntityContext<T>();

//        DynamicMethod method = new DynamicMethod("DynamicCreate", typeof(T),
//                new Type[] { typeof(IDataRecord) }, typeof(T), true);
//        ILGenerator generator = method.GetILGenerator();

//        LocalBuilder result = generator.DeclareLocal(typeof(T));
//        generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
//        generator.Emit(OpCodes.Stloc, result);

//        for (int i = 0; i < dataRecord.FieldCount; i++)
//        {/
//            PropertyInfo propertyInfo = typeof(T).GetProperty(dataRecord.GetName(i));
//            Label endIfLabel = generator.DefineLabel();

//            if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
//            {
//                generator.Emit(OpCodes.Ldarg_0);
//                generator.Emit(OpCodes.Ldc_I4, i);
//                generator.Emit(OpCodes.Callvirt, isDBNullMethod);
//                generator.Emit(OpCodes.Brtrue, endIfLabel);

//                generator.Emit(OpCodes.Ldloc, result);
//                generator.Emit(OpCodes.Ldarg_0);
//                generator.Emit(OpCodes.Ldc_I4, i);
//                generator.Emit(OpCodes.Callvirt, getValueMethod);
//                generator.Emit(OpCodes.Unbox_Any, dataRecord.GetFieldType(i));
//                generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());

//                var a = dataRecord.GetFieldType(i);

//                generator.MarkLabel(endIfLabel);
//            }
//        }

//        generator.Emit(OpCodes.Ldloc, result);
//        generator.Emit(OpCodes.Ret);

//        dynamicBuilder.handler = (Load)method.CreateDelegate(typeof(Load));
//        return dynamicBuilder;
//    }
//}
