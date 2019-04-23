using Library.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SeanAPI.DAL
{
    public class DataExtension
    {
        public SqlConnection cn;

        public DataTable ExecuteStoreProcedure(string storeProcedure, SqlParameter[] data)
        {
            DataTable table = new DataTable(storeProcedure);
            using (SqlCommand cm = new SqlCommand(storeProcedure, cn))
            {
                try
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = ConstResource.I_CONNECTION_TIME_OUT_SECS;
                    foreach (var parameter in data)
                    {
                        cm.Parameters.Add(parameter);
                    }

                    using (var da = new SqlDataAdapter(cm))
                    {
                        da.Fill(table);
                    }
                }
                catch (Exception ex) { throw ex; }
            }
            return table;
        }

        public DataTable ExecuteStoreProcedure(string storeProcedure, Hashtable data)
        {
            DataTable table = new DataTable(storeProcedure);
            using (SqlCommand cm = new SqlCommand(storeProcedure, cn))
            {
                try
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = ConstResource.I_CONNECTION_TIME_OUT_SECS;
                    foreach (DictionaryEntry parameter in data)
                    {
                        if (parameter.Value != null)
                        {
                            cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), parameter.Value));
                        }
                        else
                        {
                            cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), DBNull.Value));
                        }
                    }
                    using (var da = new SqlDataAdapter(cm))
                    {
                        da.Fill(table);
                    }
                }
                catch (Exception ex) { throw ex; }
            }
            return table;
        }

        private T Read<T>(IDataReader idr)
        {
            T retVal = Activator.CreateInstance<T>();
            var lstProperties = retVal.GetType().GetProperties();
            for (int i = 0; i < idr.FieldCount; i++)
            {
                string fieldName = idr.GetName(i);
                var property = lstProperties.FirstOrDefault(x => x.Name == fieldName);
                if (property != null)
                {
                    object value = idr.GetValue(i);
                    if (value != null && !idr.IsDBNull(i))
                    {
                        Type targetType = property.PropertyType;
                        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                        {
                            targetType = Nullable.GetUnderlyingType(property.PropertyType);
                        }
                        property.SetValue(retVal, Convert.ChangeType(value, targetType));
                    }
                }
                //else
                //{
                //     string a = ($"Field {fieldName} not in Entity");
                //}
            }
            return retVal;
        }

        /// <summary>
        /// Author:Sean<para/>
        /// Description: Execute store procedure and return a List type. <para/>
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="storeProcedure">Store procedure name</param>
        /// <param name="data">Parameters</param>
        /// <returns>Return <list type="T"></list></returns>
        public IList<T> ExecuteStoreProcedure<T>(string storeProcedure, SqlParameter[] data)
        {
            IList<T> retVal = new List<T>();
            using (SqlCommand cm = new SqlCommand(storeProcedure, cn))
            {
                try
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = ConstResource.I_CONNECTION_TIME_OUT_SECS;
                    foreach (var parameter in data)
                    {
                        cm.Parameters.Add(parameter);
                    }
                    cm.Connection.Open();
                    using (var reader = cm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(Read<T>(reader));
                        }
                    }
                    cm.Connection.Close();
                }
                catch (Exception ex) { throw ex; }
            }
            return retVal;
        }

        /// <summary>
        /// Author:Sean<para/>
        /// Description: Execute store procedure and return a List type. <para/>
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="storeProcedure">Store procedure name</param>
        /// <param name="data">Parameters contain in hastable</param>
        /// <returns>Return <list type="T"></list></returns>
        public IList<T> ExecuteStoreProcedure<T>(string storeProcedure, Hashtable data)
        {
            IList<T> retVal = new List<T>();
            using (SqlCommand cm = new SqlCommand(storeProcedure))
            {
                try
                {
                    cm.Connection = new SqlConnection(cn.ConnectionString);
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = ConstResource.I_CONNECTION_TIME_OUT_SECS;
                    foreach (DictionaryEntry parameter in data)
                    {

                        if (parameter.Value != null)
                        {
                            // Check if DataTable type
                            if (parameter.Value.GetType() == typeof(DataTable))
                            {
                                var para = new SqlParameter(parameter.Key.ToString(), parameter.Value);
                                var dt = parameter.Value as DataTable;
                                para.TypeName = dt.TableName;
                                para.SqlDbType = SqlDbType.Structured;
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
                    }
                    cm.Connection.Open();
                    using (var reader = cm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(Read<T>(reader));
                        }
                    }
                    cm.Connection.Close();
                }
                catch (Exception ex) { throw ex; }
                finally { cm.Connection.Close(); }
            }
            return retVal;
        }

        /// <summary>
        /// Sean
        /// Get Multi Table From Stored Procedure
        /// </summary>
        /// <param name="storeProcedure"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public DataSet ExecuteStoreProcedureGetMultiTables(string storeProcedure, Hashtable data)
        {
            DataSet ds = new DataSet();
            using (SqlCommand cm = new SqlCommand(storeProcedure, cn))
            {
                try
                {
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = ConstResource.I_CONNECTION_TIME_OUT_SECS;
                    foreach (DictionaryEntry parameter in data)
                    {
                        if (parameter.Value != null)
                        {
                            cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), parameter.Value));
                        }
                        else
                        {
                            cm.Parameters.Add(new SqlParameter(parameter.Key.ToString(), DBNull.Value));
                        }
                    }
                    using (var da = new SqlDataAdapter(cm))
                    {
                        da.Fill(ds);
                    }
                }
                catch (Exception ex) { throw ex; }
            }
            return ds;
        }


    }
}
