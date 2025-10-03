using Common.Constant;
using Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Reflection;

namespace Common.UnitOfWork.UnitOfWorkPattern;

public class UnitOfWork : IUnitOfWork
{
    private readonly string? _connectionString = "";
    private readonly DbContext _context;
    private bool disposed;
    private readonly Dictionary<Type, object> repositories;

    public UnitOfWork(MQTTContext dbContext)
    {
        _context = dbContext;
        repositories = new Dictionary<Type, object>();
        disposed = false;
        _connectionString = dbContext.Database.GetConnectionString();
    }

    public bool SaveAndReload<T>(T entity) where T : class
    {
        try
        {
            if (_context.SaveChanges() > 0)
            {
                _context.Entry(entity).Reload();
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public int SaveChanges()
    {
        try
        {
            return _context.SaveChanges();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public DbSet<T> Repository<T>() where T : class
    {
        return _context.Set<T>();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IQueryable<T> ExecSqlQueryRaw<T>(string storeProcedure, SqlParameter[] parameters)
    {
        string parameterList = string.Join(", ", parameters.Select(p => $"{p.ParameterName}"));
        string queryString = $"EXEC {storeProcedure} {parameterList}";
        var resFromStore = _context.Database.SqlQueryRaw<T>(queryString, parameters);
        return resFromStore;
    }

    public IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity>> ChangeTracker<TEntity>() where TEntity : class
    {
        return _context.ChangeTracker.Entries<TEntity>().ToArray();
    }

    public DataTable ExecuteStoreProcedure(string storeProcedure, SqlParameter[] data)
    {
        var table = new DataTable(storeProcedure);
        using (var cm = new SqlCommand(storeProcedure))
        {
            try
            {
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                foreach (var parameter in data) cm.Parameters.Add(parameter);

                using var da = new SqlDataAdapter(cm);
                da.Fill(table);
            }
            catch (Exception)
            {
                throw;
            }
        }

        return table;
    }

    public DataTable ExecuteStoreProcedure(string storeProcedure, Hashtable data)
    {
        var table = new DataTable(storeProcedure);
        using (var cm = new SqlCommand(storeProcedure))
        {
            try
            {
                cm.Connection = new SqlConnection(_connectionString);
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                foreach (DictionaryEntry parameter in data)
                    if (parameter.Value != null)
                        cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), parameter.Value));
                    else
                        cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), DBNull.Value));
                using var da = new SqlDataAdapter(cm);
                da.Fill(table);
            }
            catch (Exception)
            {
                throw;
            }
        }

        return table;
    }

    public IList<T> ExecuteStoreProcedure<T>(string storeProcedure, SqlParameter[] data)
    {
        IList<T> retVal = new List<T>();
        using (var cm = new SqlCommand(storeProcedure))
        {
            try
            {
                cm.Connection = new SqlConnection(_connectionString);
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                foreach (var parameter in data) cm.Parameters.Add(parameter);
                cm.Connection.Open();
                using (var reader = cm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = Read<T>(reader);
                        if (item != null) retVal.Add(item);
                    }
                }

                cm.Connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        return retVal;
    }

    public IList<T> ExecuteStoreProcedure<T>(string storeProcedure, Hashtable data)
    {
        IList<T> retVal = new List<T>();
        using (var cm = new SqlCommand(storeProcedure))
        {
            try
            {
                cm.Connection = new SqlConnection(_connectionString);
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                foreach (DictionaryEntry parameter in data)
                    if (parameter.Value != null)
                    {
                        // Check if DataTable type
                        if (parameter.Value.GetType() == typeof(DataTable))
                        {
                            var para = new SqlParameter(parameter.Key.ToString(), parameter.Value);
                            var dt = parameter.Value as DataTable;
                            if (dt != null)
                            {
                                para.TypeName = dt.TableName;
                                para.SqlDbType = SqlDbType.Structured;
                            }
                            cm.Parameters.Add(para);
                        }
                        else
                        {
                            cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), parameter.Value));
                        }
                    }
                    else
                    {
                        cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), DBNull.Value));
                    }

                cm.Connection.Open();
                using (var reader = cm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = Read<T>(reader);
                        if (item != null) retVal.Add(item);
                    }
                }

                cm.Connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cm.Connection.Close();
            }
        }

        return retVal;
    }

    public DataSet ExecuteStoreProcedureGetMultiTables(string storeProcedure, Hashtable data)
    {
        var ds = new DataSet();
        using (var cm = new SqlCommand(storeProcedure))
        {
            try
            {
                cm.Connection = new SqlConnection(_connectionString);
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                foreach (DictionaryEntry parameter in data)
                    if (parameter.Value != null)
                        cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), parameter.Value));
                    else
                        cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), DBNull.Value));
                using var da = new SqlDataAdapter(cm);
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        return ds;
    }

    public DataSet ExecuteStoreProcedureGetMultiTables(string storeProcedure, SqlParameter[] data)
    {
        var ds = new DataSet();
        using (var cm = new SqlCommand(storeProcedure))
        {
            try
            {
                cm.Connection = new SqlConnection(_connectionString);
                cm.CommandType = CommandType.StoredProcedure;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                foreach (var parameter in data) cm.Parameters.Add(parameter);
                using var da = new SqlDataAdapter(cm);
                da.Fill(ds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        return ds;
    }

    /// <summary>
    ///     execute store procedure
    /// </summary>
    /// <param name="storeProcedure">Store procedure name</param>
    /// <param name="param">Parameters, maybe is dynamic object, expando object</param>
    /// <returns></returns>
    public IEnumerable<T> QueryMultiple<T>(string storeProcedure, ExpandoObject param)
    {
        using var cm = new SqlCommand(storeProcedure);
        using (cm.Connection = new SqlConnection(_connectionString))
        {
            cm.CommandType = CommandType.StoredProcedure;
            cm.CommandTimeout = Constants.DefaultTimeExpired;
            cm.Connection.Open();
            var arrayParam = RetrieveParameter(storeProcedure, cm.Connection, param).ToArray();
            cm.Parameters.AddRange(arrayParam);

            if (typeof(T) == typeof(DataTable))
            {
                var ds = new DataSet();
                using (var da = new SqlDataAdapter(cm))
                {
                    da.Fill(ds);
                }

                foreach (var dt in ds.Tables) yield return (T)Convert.ChangeType(dt, typeof(T));
            }
            else if (typeof(T) == typeof(DataSet))
            {
                var ds = new DataSet();
                using (var da = new SqlDataAdapter(cm))
                {
                    da.Fill(ds);
                }

                yield return (T)Convert.ChangeType(ds, typeof(T));
            }
            else
            {
                var isExpandoObject = typeof(T) == typeof(ExpandoObject);
                using var reader = cm.ExecuteReader();
                while (reader.Read())
                {
                    var result = isExpandoObject ? (T)Read(reader) : Read<T>(reader);
                    if (result != null) yield return result;
                }
            }
        }
    }

    public T ExecuteScalarFunction<T>(string functionName, Dictionary<string, object> data)
    {
        functionName = functionName.Replace("dbo.", "");
        var paraName = "";
        var lstParameters = new List<SqlParameter>();
        foreach (var o in data)
        {
            paraName += $",@{o.Key.TrimStart('@')} ";
            lstParameters.Add(new SqlParameter(o.Key, o.Value));
        }

        if (paraName.Length > 0) paraName = paraName.Remove(0, 1);

        using var cm = new SqlCommand($"Select dbo.{functionName}({paraName})");
        try
        {
            cm.Connection = new SqlConnection(_connectionString);
            cm.CommandType = CommandType.Text;
            cm.CommandTimeout = Constants.DefaultTimeExpired;
            cm.Parameters.AddRange(lstParameters.ToArray());
            cm.Connection.Open();
            var result = cm.ExecuteScalar();
            cm.Connection.Close();
            return (T)result;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            cm.Connection.Close();
        }
    }

    public IList<T> ExecuteReaderFunction<T>(string functionName, Dictionary<string, object> data)
    {
        IList<T> retVal = new List<T>();
        functionName = functionName.Replace("dbo.", "");
        var paraName = "";
        var lstParameters = new List<SqlParameter>();
        foreach (var o in data)
        {
            paraName += $",@{o.Key.TrimStart('@')} ";
            lstParameters.Add(new SqlParameter(o.Key, o.Value));
        }

        if (paraName.Length > 0) paraName = paraName.Remove(0, 1);

        using (var cm = new SqlCommand($"Select * From dbo.{functionName}({paraName})"))
        {
            try
            {
                cm.Connection = new SqlConnection(_connectionString);
                cm.CommandType = CommandType.Text;
                cm.CommandTimeout = Constants.DefaultTimeExpired;
                cm.Parameters.AddRange(lstParameters.ToArray());
                cm.Connection.Open();
                using (var reader = cm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = Read<T>(reader);
                        if (item != null) retVal.Add(item);
                    }
                }

                cm.Connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                cm.Connection.Close();
            }
        }

        return retVal;
    }

    private T? Read<T>(IDataReader idr)
    {
        if (typeof(T).IsValueType)
        {
            if (idr.FieldCount > 0)
            {
                var value = idr.GetValue(0);
                if (value != null && !idr.IsDBNull(0)) return (T)value;
            }

            return default;
        }

        var retVal = Activator.CreateInstance<T>();
        if (retVal == null) return default!;
        var lstProperties = retVal.GetType().GetProperties();
        for (var i = 0; i < idr.FieldCount; i++)
        {
            var fieldName = idr.GetName(i);
            var property = lstProperties.FirstOrDefault(x => x.Name == fieldName);
            if (property != null)
            {
                var value = idr.GetValue(i);
                if (value != null && !idr.IsDBNull(i))
                {
                    var targetType = property.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                    if (underlyingType != null)
                        targetType = underlyingType;
                    property.SetValue(retVal, Convert.ChangeType(value, targetType));
                }
            }
        }

        return retVal;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            try
            {
                if (disposing)
                {
                    //if (GetSqlConnection() != null) _context.Database.CloseConnection();
                    _context?.Dispose();
                }
            }
            catch (Exception)
            {
            }

            disposed = true;
        }
    }

    //private SqlConnection GetSqlConnection()
    //{
    //    try
    //    {
    //        return (SqlConnection)_context.Database.GetDbConnection();
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }
    //}


    /// <summary>
    ///     Fill data table from list
    /// </summary>
    private void FillDataTable(DataTable dtSchema, IList iList)
    {
        if (iList != null)
        {
            var listColumns = dtSchema.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
            foreach (IDictionary<string, object> item in iList)
            {
                var row = dtSchema.NewRow();
                foreach (var columnName in listColumns) row[columnName] = GetValue(item, columnName);

                dtSchema.Rows.Add(row);
            }
        }
    }

    /// <summary>
    ///     Retrieve parameters
    /// </summary>
    /// <param name="storeProcedure">Store procedure name</param>
    /// <param name="connection">Connection</param>
    /// <param name="param">Parameters</param>
    /// <returns></returns>
    private IEnumerable<SqlParameter> RetrieveParameter(string storeProcedure, SqlConnection connection,
        ExpandoObject param)
    {
        var expandObjects = param as IDictionary<string, object>;
        var lstParas = StoreProcedureParameter.DeriveParameters(storeProcedure, out var structures);
        if (lstParas.Count == 0)
        {
            using (var cmd = new SqlCommand(storeProcedure, connection) { CommandType = CommandType.StoredProcedure })
            {
                SqlCommandBuilder.DeriveParameters(cmd);
                lstParas = cmd.Parameters.OfType<SqlParameter>().Where(x =>
                    x.ParameterName != "@RETURN_VALUE" && x.ParameterName != "@TABLE_RETURN_VALUE").ToList();
                foreach (var parameter in lstParas.Where(x => x.SqlDbType == SqlDbType.Structured))
                {
                    var tableName = parameter.TypeName.Split('.').Last();
                    var dtSchema = RetrieveParameterAsTable(tableName, connection);
                    structures.Add(parameter.ParameterName, dtSchema);
                }
            }

            StoreProcedureParameter.StoreParameters(storeProcedure, lstParas, structures);
        }

        foreach (var parameter in lstParas)
        {
            var value = GetValue(expandObjects, parameter.ParameterName);
            if (parameter.SqlDbType == SqlDbType.Structured)
            {
                var dtSchema = structures[parameter.ParameterName];
                var listValue = value as IList;
                if (listValue != null)
                {
                    FillDataTable(dtSchema, listValue);
                }
                yield return new SqlParameter(parameter.ParameterName, dtSchema)
                {
                    SqlDbType = SqlDbType.Structured,
                    TypeName = parameter.TypeName.Split('.').Last()
                };
            }
            else
            {
                yield return new SqlParameter(parameter.ParameterName, ChangeValueToSqlType(value, parameter))
                {
                    Direction = parameter.Direction,
                    DbType = parameter.DbType,
                    SqlDbType = parameter.SqlDbType,
                    Scale = parameter.Scale,
                    Size = parameter.Size,
                    Precision = parameter.Precision,
                    Offset = parameter.Offset
                };
            }
        }
    }

    /// <summary>
    ///     Retrieve parameters as table.
    /// </summary>
    /// <returns></returns>
    private DataTable RetrieveParameterAsTable(string tableName, SqlConnection connection)
    {
        using var cm = new SqlCommand("EXEC dbo.SP_GetParameterOfTableType @TableName = @Name", connection)
        { CommandType = CommandType.Text };
        var newPara = new SqlParameter("Name", SqlDbType.NVarChar, 255)
        {
            Value = tableName
        };
        cm.Parameters.Add(newPara);
        var retVal = new List<ExpandoObject>();
        using (var reader = cm.ExecuteReader())
        {
            while (reader.Read()) retVal.Add(Read(reader));
        }

        var dt = new DataTable($"Dt{TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))}{tableName}");
        foreach (dynamic o in retVal) dt.Columns.Add($"{o.name}");

        return dt;
    }

    private object ChangeValueToSqlType(object inputValue, SqlParameter targetPara)
    {
        if (inputValue == null) return DBNull.Value;
        if (inputValue == DBNull.Value) return DBNull.Value;
        switch (targetPara.SqlDbType)
        {
            case SqlDbType.BigInt:
                return Convert.ToInt64(inputValue);
            case SqlDbType.Bit:
                return Convert.ToBoolean(inputValue);
            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.NText:
            case SqlDbType.Text:
            case SqlDbType.NVarChar:
            case SqlDbType.VarChar:
                return Convert.ToString(inputValue) ?? string.Empty;
            case SqlDbType.DateTime:
            case SqlDbType.SmallDateTime:
            case SqlDbType.Date:
            case SqlDbType.DateTime2:
            case SqlDbType.DateTimeOffset:
                return Convert.ToDateTime(inputValue);
            case SqlDbType.Decimal:
                return Convert.ToDecimal(inputValue);
            case SqlDbType.Float:
                return Convert.ToDouble(inputValue);
            case SqlDbType.Int:
                return Convert.ToInt32(inputValue);
            case SqlDbType.Real:
                return Convert.ToSingle(inputValue);
            case SqlDbType.UniqueIdentifier:
                return Guid.Parse($"{inputValue}");
            case SqlDbType.SmallInt:
            case SqlDbType.TinyInt:
                return Convert.ToInt16(inputValue);
            case SqlDbType.Timestamp:
                return TimeSpan.Parse($"{inputValue}");
            case SqlDbType.Xml:
                return Convert.ToString(inputValue) ?? string.Empty;
            default:
                throw new Exception($"Not implements for sql_db_type: {targetPara.SqlDbType}");
        }
    }

    private void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
    {
        var expandoDict = expando as IDictionary<string, object>;
        if (expandoDict.ContainsKey(propertyName))
            expandoDict[propertyName] = propertyValue;
        else
            expandoDict.Add(propertyName, propertyValue);
    }

    private dynamic Read(IDataReader idr)
    {
        dynamic retVal = new ExpandoObject();

        for (var i = 0; i < idr.FieldCount; i++)
        {
            var fieldName = idr.GetName(i);
            var value = idr.GetValue(i);
            AddProperty(retVal, fieldName, value);
        }

        return retVal;
    }

    /// <summary>
    ///     Get value
    /// </summary>
    /// <returns></returns>
    private object GetValue(IDictionary<string, object> item, string fieldName)
    {
        if (item.TryGetValue(fieldName.Trim('@'), out var v))
        {
            return v == null || v == DBNull.Value ? DBNull.Value : v;
        }

        return DBNull.Value;
    }
}

public static class StoreProcedureParameter
{
    private static readonly Dictionary<string, Store> Dic = new();

    public static void StoreParameters(string storeProcedure, List<SqlParameter> lstParas,
        Dictionary<string, DataTable> structures)
    {
        if (Dic.ContainsKey(storeProcedure))
            Dic.Remove(storeProcedure);

        Dic.Add(storeProcedure, new Store(storeProcedure, lstParas, structures));
    }

    public static List<SqlParameter> DeriveParameters(string storeProcedure,
        out Dictionary<string, DataTable> structures)
    {
        if (Dic.ContainsKey(storeProcedure))
        {
            var store = Dic[storeProcedure];
            structures = store.GetStructures();
            return store.GetSqlParameters().ToList();
        }

        structures = new Dictionary<string, DataTable>();
        return new List<SqlParameter>();
    }

    public static List<T> ToList<T>(this DataTable dataTable)
    {
        var dataList = new List<T>();

        //-- If T is value type
        if (typeof(T).IsValueType || typeof(T) == typeof(string))
            return dataTable.Rows.Cast<DataRow>().Select(x => (T)x[0]).ToList();

        //Define what attributes to be read from the class
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

        //Read Attribute Names and Types
        var objFieldNames = typeof(T).GetProperties(flags).Select(item => new
        {
            item.Name,
            Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
        }).ToList();

        //Read Datatable column names and types
        var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().Select(item => new
        {
            Name = item.ColumnName,
            Type = item.DataType
        }).ToList();

        foreach (DataRow dataRow in dataTable.Rows)
        {
            var classObj = Activator.CreateInstance<T>();
            if (classObj == null) continue;

            foreach (var dtField in dtlFieldNames)
            {
                if (string.IsNullOrEmpty(dtField.Name)) continue;
                
                var propertyInfos = classObj.GetType().GetProperty(dtField.Name);
                var field = objFieldNames.Find(x => x.Name == dtField.Name);
                if (field != null && propertyInfos != null)
                {
                    if (propertyInfos.PropertyType.IsGenericType &&
                        (propertyInfos.PropertyType.Name.Contains("Nullable") ||
                         propertyInfos.PropertyType.Name.Contains("?")))
                    {
                        if (!string.IsNullOrEmpty(dataRow[dtField.Name]?.ToString()) && dataRow[dtField.Name] != DBNull.Value)
                        {
                            var underlyingType = Nullable.GetUnderlyingType(propertyInfos.PropertyType);
                            if (underlyingType != null)
                            {
                                propertyInfos.SetValue(classObj, Convert.ChangeType(dataRow[dtField.Name],
                                    underlyingType, null));
                            }
                        }
                        //else do nothing
                    }
                    else
                    {
                        if (dataRow[dtField.Name] != DBNull.Value)
                        {
                            propertyInfos.SetValue(classObj,
                                Convert.ChangeType(dataRow[dtField.Name], propertyInfos.PropertyType), null);
                        }
                    }
                }
            }

            dataList.Add(classObj);
        }

        return dataList;
    }

    private static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
    {
        var expandoDict = expando as IDictionary<string, object>;
        if (expandoDict.ContainsKey(propertyName))
            expandoDict[propertyName] = propertyValue;
        else
            expandoDict.Add(propertyName, propertyValue);
    }

    private static dynamic Read(IDataReader idr)
    {
        dynamic retVal = new ExpandoObject();

        for (var i = 0; i < idr.FieldCount; i++)
        {
            var fieldName = idr.GetName(i);
            var value = idr.GetValue(i);
            AddProperty(retVal, fieldName, value);
        }

        return retVal;
    }

    public static IEnumerable<dynamic> Read(this DataTable table)
    {
        using var reader = table.CreateDataReader();
        while (reader.Read()) yield return Read(reader);
    }

    private class Store
    {
        public Store(string procedureName, IEnumerable<SqlParameter> paras, Dictionary<string, DataTable> structures)
        {
            ProcedureName = procedureName;
            Parameters = paras.Select(GetOne).ToList();
            Structures = structures.ToDictionary(x => x.Key, x => GetOne(x.Value));
        }

        private string ProcedureName { get; }
        private List<SqlParameter> Parameters { get; }
        private Dictionary<string, DataTable> Structures { get; }

        public Dictionary<string, DataTable> GetStructures()
        {
            return Structures.ToDictionary(x => x.Key, x => GetOne(x.Value));
        }

        public IEnumerable<SqlParameter> GetSqlParameters()
        {
            return Parameters.Select(GetOne);
        }

        private static SqlParameter GetOne(SqlParameter x)
        {
            return new SqlParameter
            {
                ParameterName = x.ParameterName,
                Size = x.Size,
                TypeName = x.TypeName,
                DbType = x.DbType,
                SqlDbType = x.SqlDbType,
                UdtTypeName = x.UdtTypeName,
                Offset = x.Offset,
                Direction = x.Direction,
                IsNullable = x.IsNullable,
                Scale = x.Scale,
                Precision = x.Precision
            };
        }

        private static DataTable GetOne(DataTable x)
        {
            var dt = new DataTable(x.TableName);
            foreach (DataColumn c in x.Columns) dt.Columns.Add(c.ColumnName);
            return dt;
        }
    }
}

public static class UtilsExtension
{
    #region For dynamic

    /// <summary>
    ///     Convert json string to expando object.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static dynamic? JsonToExpandoObject(string json)
    {
        return JsonConvert.DeserializeObject<ExpandoObject>(json, new ExpandoObjectConverter());
    }

    public static string ToJsonString(object data)
    {
        return JsonConvert.SerializeObject(data);
    }

    #endregion
}

public static class DataSetExtensions
{
    public static DataSetSimpleRead ToDataSetSimpleRead(this DataSet dataSet)
    {
        return new DataSetSimpleRead(dataSet);
    }
}

public class DataSetSimpleRead
{
    private readonly IEnumerator _enumerator;

    public DataSetSimpleRead(DataSet dataSet)
    {
        _enumerator = dataSet.Tables.GetEnumerator();
    }

    /// <summary>
    ///     Read current table, after move to next node. Throw if can't move to next node.
    /// </summary>
    /// <returns></returns>
    public List<T> Read<T>()
    {
        return _enumerator.MoveNext() ? ((DataTable)_enumerator.Current).ToList<T>() : throw new Exception("Index of table out of range");
    }

    /// <summary>
    ///     Read current table, after move to next node. throw if can't move to next node.
    /// </summary>
    /// <returns>Return dynamic object</returns>
    public List<dynamic> Read()
    {
        if (_enumerator.MoveNext())
        {
            var dt = (DataTable)_enumerator.Current;
            return dt.Read().ToList();
        }

        throw new Exception("Index of table out of range");
    }

    public DataTable ReadAsTable()
    {
        if (_enumerator.MoveNext())
        {
            var dt = (DataTable)_enumerator.Current;
            return dt;
        }

        throw new Exception("Index of table out of range");
    }

    public DataTable? TryReadAsTable()
    {
        try
        {
            return ReadAsTable();
        }
        catch
        {
        }

        return null;
    }

    public List<T>? TryRead<T>()
    {
        try
        {
            return Read<T>();
        }
        catch
        {
        }

        return null;
    }

    public List<dynamic>? TryRead()
    {
        try
        {
            return Read();
        }
        catch
        {
        }

        return null;
    }
}
