using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using Telerik.OpenAccess.Data.Common;

namespace MVCLMS.Data_Access_Layer
{
    public static class DataAccessLayer
    {
        public static IEnumerable<T> ExecuteQuery<T>(string ProcedureName, Dictionary<string, string> parameters = null)
        {
            IEnumerable<T> ReturnResult;

            using (DBModel dbContext = new DBModel())
            {
                if (parameters != null)
                {
                    List<DbParameter> paramters_items = new List<DbParameter>();

                    foreach (KeyValuePair<string, string> item in parameters)
                    {
                        DbParameter NewParameter = new OAParameter
                        {
                            ParameterName = item.Key,
                            Value = item.Value
                        };

                        paramters_items.Add(NewParameter);
                    }

                    DbParameter[] dbparams = paramters_items.ToArray();
                    ReturnResult = dbContext.ExecuteQuery<T>(ProcedureName, CommandType.StoredProcedure, dbparams);
                }
                else
                {
                    ReturnResult = dbContext.ExecuteQuery<T>(ProcedureName, CommandType.StoredProcedure);
                }
            }

            return ReturnResult;
        }

        public static int ExecuteNonQueryWithReturnValue(string ProcedureName, Dictionary<string, string> parameters = null)
        {
            int ReturnValue = 0;

            using (DBModel dbContext = new DBModel())
            {
                using (IDbConnection dbConnection = dbContext.Connection)
                {
                    using (IDbCommand spCommand = dbConnection.CreateCommand())
                    {
                        spCommand.CommandText = ProcedureName;
                        spCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            foreach (KeyValuePair<string, string> item in parameters)
                            {
                                IDbDataParameter NewParameter = spCommand.CreateParameter();
                                NewParameter.ParameterName = item.Key;
                                NewParameter.Value = item.Value;
                                spCommand.Parameters.Add(NewParameter);
                            }
                        }

                        IDbDataParameter ReturnValueParamter = spCommand.CreateParameter();
                        ReturnValueParamter.Direction = ParameterDirection.ReturnValue;
                        spCommand.Parameters.Add(ReturnValueParamter);

                        spCommand.ExecuteNonQuery();
                        dbContext.SaveChanges();

                        ReturnValue = Convert.ToInt32(ReturnValueParamter.Value);
                    }
                }
            }
            return ReturnValue;
        }

        public static void ExecuteNonQuery(string ProcedureName, Dictionary<string, string> parameters = null)
        {
            using (DBModel dbContext = new DBModel())
            {
                using (IDbConnection dbConnection = dbContext.Connection)
                {
                    using (IDbCommand spCommand = dbConnection.CreateCommand())
                    {
                        spCommand.CommandText = ProcedureName;
                        spCommand.CommandType = System.Data.CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            foreach (KeyValuePair<string, string> item in parameters)
                            {
                                IDbDataParameter NewParameter = spCommand.CreateParameter();
                                NewParameter.ParameterName = item.Key;
                                NewParameter.Value = item.Value;
                                spCommand.Parameters.Add(NewParameter);
                            }
                        }

                        spCommand.ExecuteNonQuery();
                        dbContext.SaveChanges();
                    }
                }
            }
        }

    }
}
