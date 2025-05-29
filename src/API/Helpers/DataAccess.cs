using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using ARCRM.Pidgets.MultiUserDiaryAPI.Models;

namespace ARCRM.Pidgets.MultiUserDiaryAPI.Helpers
{
    public class DataAccess : IDisposable
    {

        #region Variables

        private SqlConnection m_Connection;
        private bool m_DisposedValue = false;

        #endregion

        #region Constants

        private const string c_SQL_TABLE_EXISTS = "SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TABLENAME";
        private const string c_SQL_COLUMN_EXISTS = "SELECT TABLE_NAME,COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = @COLUMNNAME AND TABLE_NAME = @TABLENAME";
        private const string c_SQL_SP_EXISTS = "SELECT COUNT(SPECIFIC_NAME) FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND SPECIFIC_NAME = @SPNAME";
        private const string c_SQL_FN_EXISTS = "SELECT COUNT(SPECIFIC_NAME) FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'FUNCTION' AND SPECIFIC_NAME = @SPNAME";
        private const string c_SQL_VIEW_EXISTS = "SELECT COUNT(TABLE_NAME) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = @VIEWNAME";
        private const string c_SQL_IDENTITY = "SELECT ISNULL(SCOPE_IDENTITY(),0)";
        private const string c_SQL_TRAN_COUNT = "SELECT @@TRANCOUNT";
        private const string c_CON_STRING_NO_WIN = "Server={0};Database={1};Uid={2};Pwd={3};TrustServerCertificate=True";
        private const string c_CON_STRING_WIN = "Server={0};Database={1};Trusted_Connection=Yes";
        private const string c_SQL_DB_EXISTS = "SELECT COUNT([NAME]) AS [EXISTS] FROM SYS.DATABASES WHERE [NAME] = @DBNAME";
        private const string c_SQL_GET_DB_LIST = "SELECT NAME FROM SYS.databases WITH(NOLOCK) WHERE DATABASE_ID > 4 ORDER BY NAME";
        private const string c_SQL_GET_SP_LIST = "SELECT SPECIFIC_NAME FROM INFORMATION_SCHEMA.ROUTINES WITH(NOLOCK) WHERE ROUTINE_TYPE = 'PROCEDURE' ORDER BY SPECIFIC_NAME";
        private const string c_SQL_GET_SP_PARAM_LIST = "SELECT PARAMETER_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = @SPNAME";
        private const string c_SQL_GET_VIEW_LIST = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS ORDER BY TABLE_NAME";
        private const string c_SQL_WFF_SEQUENCE_SP = "TSSP_GET_SEQUENCE";

        private const int c_CMDTimeOut = 600;

        #endregion

        #region Properties

        protected internal string SQLDataBaseName { get; set; } = "";
        protected internal string SQLServerName { get; set; } = "";
        protected internal string SQLUserName { get; set; } = "";
        protected internal string SQLPassword { get; set; } = "";
        protected internal bool SQLWinAuth { get; set; } = false;
        protected internal SqlTransaction Transaction { get; private set; } = null;

        #endregion

        #region Constructor

        public DataAccess(string pConnectionString)
        {

            var connectionBuilder = new SqlConnectionStringBuilder(pConnectionString);
            SQLDataBaseName = connectionBuilder.InitialCatalog;
            SQLServerName = connectionBuilder.DataSource;
            SQLUserName = connectionBuilder.UserID;
            SQLWinAuth = connectionBuilder.IntegratedSecurity;
            SQLPassword = connectionBuilder.Password;
        }


        #endregion

        #region Connection Methods

        private bool OpenConnection()
        {

            // Opens a connection connection string
            string DBName;
            string ConnectionString;

            try
            {
                if (m_Connection == null)
                    m_Connection = new SqlConnection();

                // Set to master if no supplied
                if (string.IsNullOrEmpty(SQLDataBaseName))
                    DBName = "master";
                else
                    DBName = SQLDataBaseName;

                // Set connection string based on win auth
                if (SQLWinAuth == false)
                    ConnectionString = string.Format(c_CON_STRING_NO_WIN, SQLServerName, DBName, SQLUserName, SQLPassword);
                else
                    ConnectionString = string.Format(c_CON_STRING_WIN, SQLServerName, DBName);

                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.ConnectionString = ConnectionString;
                    m_Connection.Open(SqlConnectionOverrides.OpenWithoutRetry);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected internal bool TestConnection()
        {

            // Test Opens a connection
            string DBName;
            string ConnectionString;

            try
            {
                if (m_Connection == null)
                    m_Connection = new SqlConnection();

                // Set to master if no supplied
                if (string.IsNullOrEmpty(SQLDataBaseName))
                    DBName = "master";
                else
                    DBName = SQLDataBaseName;

                // Set connection string based on win auth
                if (SQLWinAuth == false)
                    ConnectionString = string.Format(c_CON_STRING_NO_WIN, SQLServerName, DBName, SQLUserName, SQLPassword);
                else
                    ConnectionString = string.Format(c_CON_STRING_WIN, SQLServerName, DBName);

                if (m_Connection.State == ConnectionState.Closed)
                {
                    m_Connection.ConnectionString = ConnectionString;
                    m_Connection.Open();
                    m_Connection.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CloseConnection()
        {
            // Closes Connection
            if (m_Connection.State != ConnectionState.Closed)
                m_Connection.Close();
        }

        protected internal void BeginTransaction()
        {
            BeginTransaction(IsolationLevel.ReadCommitted);
        }

        protected internal void BeginTransaction(IsolationLevel type)
        {
            OpenConnection();
            Transaction = m_Connection.BeginTransaction(type);
        }

        protected internal void CommitTransaction()
        {
            int tranCount = Convert.ToInt32(GetSingleValue(c_SQL_TRAN_COUNT));
            if (!(Transaction == null) & tranCount > 0)
                Transaction.Commit();
            Transaction = null;
            CloseConnection();
        }

        protected internal void RollbackTransaction()
        {
            int tranCount = Convert.ToInt32(GetSingleValue(c_SQL_TRAN_COUNT));
            if (!(Transaction == null) & tranCount > 0)
                Transaction.Rollback();
            Transaction = null;
            CloseConnection();
        }

        #endregion

        #region Data Methods

        // Get dataset
        protected internal DataSet GetDataSet(string strSQL, List<SqlParameter> Parameters = null)
        {
            DataSet GetDataSetRet;
            if (Transaction == null)
                OpenConnection();
            try
            {
                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = strSQL,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlAdapter.SelectCommand.Parameters.Add(sqlParameter);
                }

                GetDataSetRet = new DataSet();
                sqlAdapter.Fill(GetDataSetRet);
                sqlAdapter.SelectCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return GetDataSetRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Get data table
        protected internal DataTable GetDataTable(string strSQL, List<SqlParameter> Parameters = null)
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = strSQL,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlAdapter.SelectCommand.Parameters.Add(sqlParameter);
                }

                var dtTable = new DataTable();

                sqlAdapter.Fill(dtTable);
                sqlAdapter.SelectCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();

                return dtTable;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Get single value
        protected internal object GetSingleValue(string strSQL, List<SqlParameter> Parameters = null)
        {
            try
            {
                object GetSingleValueRet;
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = strSQL,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };

                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlCommand.Parameters.Add(sqlParameter);
                }

                GetSingleValueRet = sqlCommand.ExecuteScalar();
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return GetSingleValueRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Get single value
        protected internal DataSet ExecuteQuery(string strSQL, List<SqlParameter> Parameters = null)
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = strSQL,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };
                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlAdapter.SelectCommand.Parameters.Add(sqlParameter);
                }

                var dsResult = new DataSet();
                sqlAdapter.Fill(dsResult);

                if (Transaction == null)
                    CloseConnection();
                return dsResult;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Execute stored procedure
        protected internal SqlParameterCollection ExecStoredProcedure(string SPName, List<SqlParameter> Parameters)
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = SPName,
                    CommandType = CommandType.StoredProcedure,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };

                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlCommand.Parameters.Add(sqlParameter);
                }
                sqlCommand.ExecuteNonQuery();
                if (Transaction == null)
                    CloseConnection();
                return sqlCommand.Parameters;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Execute stored procedure return results
        protected internal DataSet ExecStoredProcedureReturnResultsDS(string SPName, List<SqlParameter> Parameters = null)
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = SPName,
                        CommandType = CommandType.StoredProcedure,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlAdapter.SelectCommand.Parameters.Add(sqlParameter);
                }

                var dtTable = new DataSet();
                sqlAdapter.Fill(dtTable);

                if (Transaction == null)
                    CloseConnection();
                sqlAdapter.SelectCommand.Parameters.Clear();
                return dtTable;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Execute stored procedure return results
        protected internal DataTable ExecStoredProcedureReturnResultsDT(string SPName, List<SqlParameter> Parameters = null)
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = SPName,
                        CommandType = CommandType.StoredProcedure,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                if (!(Parameters == null))
                {
                    foreach (SqlParameter sqlParameter in Parameters)
                        sqlAdapter.SelectCommand.Parameters.Add(sqlParameter);
                }

                var dtTable = new DataTable();
                sqlAdapter.Fill(dtTable);
                sqlAdapter.SelectCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return dtTable;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }


        // Execute SQL command
        protected internal void ExecCommand(SqlCommand command)
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                {
                    var withBlock = command;
                    withBlock.Transaction = Transaction;
                    withBlock.Connection = m_Connection;
                    withBlock.CommandTimeout = c_CMDTimeOut;
                    withBlock.ExecuteNonQuery();
                }

                if (Transaction == null)
                    CloseConnection();
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Execute SQL statement
        protected internal int ExecSQL(string strSQL, List<SqlParameter> SelectParameters = null)
        {
            var result = 0;
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = strSQL,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };

                if (!(SelectParameters == null))
                {
                    foreach (SqlParameter sqlParameter in SelectParameters)
                        sqlCommand.Parameters.Add(sqlParameter);
                }
                result = sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();

                return result;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Execute SQL and return identity
        protected internal long ExecSQLReturnIdentity(string strSQL, List<SqlParameter> SelectParameters = null)
        {
            try
            {
                long ExecSQLReturnIdentityRet;
                if (Transaction == null)
                    OpenConnection();

                // Set identity
                strSQL = strSQL + ";" + c_SQL_IDENTITY;

                var sqlCommand = new SqlCommand()
                {
                    CommandText = strSQL,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };

                if (!(SelectParameters == null))
                {
                    foreach (SqlParameter sqlParameter in SelectParameters)
                        sqlCommand.Parameters.Add(sqlParameter);
                }
                ExecSQLReturnIdentityRet = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return ExecSQLReturnIdentityRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Check if SP exists
        protected internal bool DoesSPExist(string SPName)
        {
            try
            {
                bool DoesSPExistRet;
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = c_SQL_SP_EXISTS,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "@SPNAME", SqlDbType = SqlDbType.VarChar, Value = SPName });

                DoesSPExistRet = Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0 ? false : true;
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return DoesSPExistRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Check if SP exists
        protected internal bool DoesFNExist(string FNName)
        {
            try
            {
                bool DoesFNExistRet;
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = c_SQL_FN_EXISTS,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "@SPNAME", SqlDbType = SqlDbType.VarChar, Value = FNName });

                DoesFNExistRet = Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0 ? false : true;
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return DoesFNExistRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }


        // Check if view exists
        protected internal bool DoesViewExist(string ViewName)
        {
            try
            {
                bool DoesViewExistRet;
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = c_SQL_VIEW_EXISTS,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "@VIEWNAME", SqlDbType = SqlDbType.VarChar, Value = ViewName });

                DoesViewExistRet = Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0 ? false : true;
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return DoesViewExistRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Check if table exists
        protected internal bool DoesTableExist(string TableName)
        {
            try
            {
                bool DoesTableExistRet;
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = c_SQL_TABLE_EXISTS,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "@TABLENAME", SqlDbType = SqlDbType.VarChar, Value = TableName });

                DoesTableExistRet = Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0 ? false : true;
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return DoesTableExistRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Check if column exists




        // Get database List
        protected internal DataTable GetDBList()
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = c_SQL_GET_DB_LIST,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                var dtTable = new DataTable();

                sqlAdapter.Fill(dtTable);
                sqlAdapter.SelectCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return dtTable;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Check if database exists
        protected internal bool DoesDBExist(string dbName)
        {
            try
            {
                bool DoesDBExistRet;
                if (Transaction == null)
                    OpenConnection();

                var sqlCommand = new SqlCommand()
                {
                    CommandText = c_SQL_DB_EXISTS,
                    Connection = m_Connection,
                    Transaction = Transaction,
                    CommandTimeout = c_CMDTimeOut
                };
                sqlCommand.Parameters.Add(new SqlParameter() { ParameterName = "@DBNAME", SqlDbType = SqlDbType.VarChar, Value = dbName });

                DoesDBExistRet = Convert.ToInt32(sqlCommand.ExecuteScalar()) == 0 ? false : true;
                sqlCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();
                return DoesDBExistRet;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }

        // Get Next Sequence
        protected internal long GetNextWFFSequence(string SequenceName)
        {

            List<SqlParameter> lstParams;
            SqlParameterCollection spcReturn;

            try
            {
                if (Transaction == null)
                    OpenConnection();

                // Create parameters
                lstParams = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@TABLE_CODE", SqlDbType = SqlDbType.VarChar, Value = SequenceName },
                new SqlParameter() {ParameterName = "@INCREMENT", SqlDbType = SqlDbType.Int, Value = 1 },
                new SqlParameter() {ParameterName = "@SEQUENCE", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output, Value = 1 }
            };

                spcReturn = ExecStoredProcedure(c_SQL_WFF_SEQUENCE_SP, lstParams);
                if (spcReturn != null)
                    return Convert.ToInt32(spcReturn["@SEQUENCE"].Value);
                else
                    return 0;
            }
            finally
            {
                if (Transaction == null)
                    CloseConnection();

            }
        }


        #endregion

        #region SQL Object Methods

        // Get sp list List
        protected internal DataTable GetSPList()
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = c_SQL_GET_SP_LIST,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                var dtTable = new DataTable();

                sqlAdapter.Fill(dtTable);
                sqlAdapter.SelectCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();

                return dtTable;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();

                throw;
            }
        }

        // Get sp parameter list
        protected internal DataTable GetSPParameterList(string SPName)
        {

            List<SqlParameter> lstParams;

            try
            {


                if (Transaction == null)
                    OpenConnection();

                // Create parameters
                lstParams = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName = "@SPNAME", SqlDbType = SqlDbType.VarChar, Value = SPName}
            };

                return GetDataTable(c_SQL_GET_SP_PARAM_LIST, lstParams);

            }
            finally
            {
                if (Transaction == null)
                    CloseConnection();

            }
        }

        // Get sp list List
        protected internal DataTable GetViewList()
        {
            try
            {
                if (Transaction == null)
                    OpenConnection();

                var sqlAdapter = new SqlDataAdapter()
                {
                    SelectCommand = new SqlCommand()
                    {
                        CommandText = c_SQL_GET_VIEW_LIST,
                        Connection = m_Connection,
                        Transaction = Transaction,
                        CommandTimeout = c_CMDTimeOut
                    }
                };

                var dtTable = new DataTable();

                sqlAdapter.Fill(dtTable);
                sqlAdapter.SelectCommand.Parameters.Clear();

                if (Transaction == null)
                    CloseConnection();

                return dtTable;
            }
            catch
            {
                if (Transaction == null)
                    CloseConnection();
                throw;
            }
        }


        #endregion

        #region Private Methods
        private T[] GetListFromDS<T>(DataSet pDataSet, int pTableIndex) where T : new()
        {
            if (pDataSet != null && pDataSet.Tables.Count > pTableIndex)
            {
                var result = pDataSet.Tables[pTableIndex].ToList<T>();
                if (result != null)
                {
                    return result.ToArray();
                }
            }
            return null;
        }

        public Dictionary<string, Type> GetColumnTypesForView(string pViewName)
        {
            var columnTypes = new Dictionary<string, Type>();
            var dataSet = ExecuteQuery("SELECT TOP 1 * FROM "+pViewName);
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                foreach (DataColumn col in dataSet.Tables[0].Columns)
                {
                    if (!columnTypes.ContainsKey(col.ColumnName) && col.ColumnName.ToLower() != "row")
                    {
                        columnTypes.Add(col.ColumnName, col.DataType);
                    }
                }
            }
            return columnTypes;
        }


        #endregion
        #region Trace Methods

        // Get SQL command text as string
        protected internal static string GetSQLCommandAsString(string SQL, List<SqlParameter> Params)
        {

            // Loop all parameters
            foreach (SqlParameter sqlParam in Params)
            {
                string strQuotes = "";

                // Check if quotes needed
                switch (sqlParam.SqlDbType)
                {
                    case var @case when @case == SqlDbType.Char:
                    case var case1 when case1 == SqlDbType.NChar:
                    case var case2 when case2 == SqlDbType.NText:
                    case var case3 when case3 == SqlDbType.NVarChar:
                    case var case4 when case4 == SqlDbType.Text:
                    case var case5 when case5 == SqlDbType.Time:
                    case var case6 when case6 == SqlDbType.VarChar:
                    case var case7 when case7 == SqlDbType.Xml:
                    case var case8 when case8 == SqlDbType.Date:
                    case var case9 when case9 == SqlDbType.DateTime:
                    case var case10 when case10 == SqlDbType.DateTime2:
                    case var case11 when case11 == SqlDbType.DateTimeOffset:
                        {
                            strQuotes = "'";
                            break;
                        }
                }

                // Replace parameter with value
                SQL = SQL.Replace(sqlParam.ParameterName, strQuotes + sqlParam.Value.ToString() + strQuotes);
            }

            return SQL;
        }




        #endregion

        #region Dispose Methods

        // IDispose
        protected virtual void Dispose(bool disposing)
        {

            // Check dispose already
            if (m_DisposedValue == false)
            {
                if (m_Connection != null)
                {
                    m_Connection.Close();
                    SqlConnection.ClearPool(m_Connection);
                    m_Connection.Dispose();
                }
            }

            // Set as disposed
            m_DisposedValue = true;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion



    }
}
