﻿using System;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Threading;
using LogManager;
using log4net;

namespace ks.Models
{
    /// <summary>
    /// Summary description for SqlDataAccess.
    /// </summary>
    public class SqlDataAccess : IDataAccess
    {
        ILogManager logger = new LogManager.LogManager();
        public SqlDataAccess()
        {
            logger.createLogger();
        }

        /**********************************************************
        Function:			ExecuteNonQuery	
        Created By:			Abhilash Rana
        Created On:			23-August-2010
        Modified By:		NIL
        Modified On:		NIL
        Purpose:			This method is used to execute the sql query
        No of Arguments:    1
        Returns:			int
        Notes:			    NIL
        ***********************************************************/
        public int ExecuteNonQuery(string cmdText)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet dsDataSet = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                DateTime dtStart = DateTime.Now;

                Int32 iReturn = oCmd.ExecuteNonQuery();

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                return iReturn;
            }

            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        public int ExecuteNonQuery(string cmdText, string[,] arrParam)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet dsDataSet = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                    oCmd.Parameters.AddWithValue(arrParam[i, 0], arrParam[i, 1]);

                Int32 iReturn = oCmd.ExecuteNonQuery();

                return iReturn;
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        public string ExecuteNonQuery(string cmdText, string[,] arrParam, string output, string dbtype, int size)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            SqlDataReader dsReader = null;

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                    oCmd.Parameters.AddWithValue(arrParam[i, 0], arrParam[i, 1]);

                SqlParameter sqlparam;
                if (dbtype == "int")
                    sqlparam = new SqlParameter(output, SqlDbType.Int);
                else
                    sqlparam = new SqlParameter(output, SqlDbType.VarChar, size);

                sqlparam.Direction = ParameterDirection.Output;
                oCmd.Parameters.Add(sqlparam);

                oCmd.ExecuteNonQuery();

                if (dbtype == "int")
                    return Convert.ToString((int)sqlparam.Value);
                else
                    return (string)sqlparam.Value;

            }


            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }
        /**********************************************************
       Function:			ExecuteScalar	
       Created By:			Abhilash Rana
       Created On:			23-August-2010
       Modified By:		    NIL
       Modified On:		    NIL
       Purpose:			    This method is used to run the sql query and return the result in a DataTable.
       No of Arguments:     1
       Returns:			    DataTable
       Notes:			    NIL
       ***********************************************************/
        public object ExecuteScalar(string cmdText)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet dsDataSet = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                DateTime dtStart = DateTime.Now;
                object oReturn = oCmd.ExecuteScalar();

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                return oReturn;
            }

            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        /**********************************************************
      Function:			    ExecuteReader	
      Created By:			Abhilash Rana
      Created On:			23-August-2010
      Modified By:		    NIL
      Modified On:		    NIL
      Purpose:			    This method is used to execute Stored Proc and return in a DataReader.
      No of Arguments:      2
      Returns:			    DataReader
      Notes:			    NIL
      ***********************************************************/

        public DataTableReader ExecuteReader(string cmdText, string[,] arrParam, bool IsWithType)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            SqlDataReader dsReader = null;
            DataTable dtTable = new DataTable();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                if (IsWithType)
                {
                    for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                        if (arrParam[i, 2].ToLower() == "string")
                            oCmd.Parameters.AddWithValue(arrParam[i, 0], arrParam[i, 1]);
                        else
                            oCmd.Parameters.AddWithValue(arrParam[i, 0], Convert.ToInt32(arrParam[i, 1]));

                }
                else
                {
                    for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                        oCmd.Parameters.AddWithValue(arrParam[i, 0], Convert.ToInt32(arrParam[i, 1]));
                }
                DateTime dtStart = DateTime.Now;

                dsReader = oCmd.ExecuteReader();

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                int iNumber = dsReader.FieldCount;

                for (int i = 0; i < iNumber; i++)
                {
                    dtTable.Columns.Add(dsReader.GetName(i));
                }

                while (dsReader.Read())
                {
                    DataRow dr = dtTable.NewRow();
                    for (int i = 0; i < iNumber; i++)
                    {
                        dr[i] = dsReader[i];
                    }
                    dtTable.Rows.Add(dr);
                }

                return dtTable.CreateDataReader();

            }


            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }
        public DataTableReader ExecuteReader(string cmdText, string[,] arrParam)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            SqlDataReader dsReader = null;
            DataTable dtTable = new DataTable();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                    oCmd.Parameters.AddWithValue(arrParam[i, 0], arrParam[i, 1]);
                DateTime dtStart = DateTime.Now;

                dsReader = oCmd.ExecuteReader();

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                int iNumber = dsReader.FieldCount;

                for (int i = 0; i < iNumber; i++)
                {
                    dtTable.Columns.Add(dsReader.GetName(i));
                }

                while (dsReader.Read())
                {
                    DataRow dr = dtTable.NewRow();
                    for (int i = 0; i < iNumber; i++)
                    {
                        dr[i] = dsReader[i];
                    }
                    dtTable.Rows.Add(dr);
                }

                return dtTable.CreateDataReader();

            }


            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }

        /**********************************************************
    Function:			    ExecuteReader	
    Created By:			Abhilash Rana
    Created On:			23-August-2010
    Modified By:		    NIL
    Modified On:		    NIL
    Purpose:			    This method is used to execute Stored Proc and return in a DataReader.
    No of Arguments:      2
    Returns:			    DataReader
    Notes:			    NIL
    ***********************************************************/
        public DataTableReader ExecuteReader(string cmdText)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            SqlDataReader dsReader = null;
            DataTable dtTable = new DataTable();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                DateTime dtStart = DateTime.Now;

                dsReader = oCmd.ExecuteReader();

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                int iNumber = dsReader.FieldCount;

                for (int i = 0; i < iNumber; i++)
                {
                    dtTable.Columns.Add(dsReader.GetName(i));
                }

                while (dsReader.Read())
                {
                    DataRow dr = dtTable.NewRow();
                    for (int i = 0; i < iNumber; i++)
                    {
                        dr[i] = dsReader[i];
                    }
                    dtTable.Rows.Add(dr);
                }

                return dtTable.CreateDataReader();

            }


            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }

        /**********************************************************
       Function:			ExecuteDataSet	
       Created By:			Abhilash Rana
       Created On:			23-August-2010
       Modified By:		    NIL
       Modified On:		    NIL
       Purpose:			    This method is used to execute Stored Proc and return first DataTable in a DataSet.
       No of Arguments:     2
       Returns:			    DataTable
       Notes:			    NIL
       ***********************************************************/
        public DataTable ExecuteDataTable(string cmdText, string[,] arrParam)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet ds = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                    oCmd.Parameters.AddWithValue(arrParam[i, 0], arrParam[i, 1]);

                DateTime dtStart = DateTime.Now;

                SqlDataAdapter daDataAdapter = new SqlDataAdapter(oCmd);
                daDataAdapter.Fill(ds);

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                if (ds != null)
                    return ds.Tables[0];
                else
                    return null;

            }


            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }
        public DataTable ExecuteDataTable(string cmdText)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet ds = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.Text;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                DateTime dtStart = DateTime.Now;

                SqlDataAdapter daDataAdapter = new SqlDataAdapter(oCmd);
                daDataAdapter.Fill(ds);

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                if (ds != null)
                    return ds.Tables[0];
                else
                    return null;

            }


            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }

        }
        public SqlDataReader ExecuteSqlCommand(string theCmd, SqlConnection theConnection)
        {

            /*  Purpose:To execute Sqlcommand by passing the commandText string along with the open connection string.
                Parameters: Command Text and the Connection String.
                Return values: returns Reader
                Process: The commandtext and connection string is passed to the sqlcommand and ExecuterReader method is called 
                which executes the query and returns SqlDataReader.
             */


            SqlDataReader reader = null;
            SqlCommand command = new SqlCommand(theCmd, theConnection);
            try
            {

                reader = command.ExecuteReader();
                //tn.Commit();
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }

            //tn.Dispose();
            return reader;
        }
        /**********************************************************
       Function:			ExecuteDataSet	
       Created By:			Abhilash Rana
       Created On:			23-August-2010
       Modified By:		    NIL
       Modified On:		    NIL
       Purpose:			    This method is used to execute Stored Proc and return results in a DataSet.
       No of Arguments:     2
       Returns:			    DataSet
       Notes:			    NIL
       ***********************************************************/
        public DataSet ExecuteDataSet(string cmdText, string[,] arrParam)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet ds = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.StoredProcedure;
                oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);

                for (Int16 i = 0; i < arrParam.GetLength(0); i++)
                    oCmd.Parameters.AddWithValue(arrParam[i, 0], arrParam[i, 1]);

                DateTime dtStart = DateTime.Now;

                SqlDataAdapter daDataAdapter = new SqlDataAdapter(oCmd);
                daDataAdapter.Fill(ds);

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                return ds;

            }


            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        /**********************************************************
      Function:			    ExecuteDataSet	
      Created By:			Abhilash Rana
      Created On:			23-August-2010
      Modified By:		    NIL
      Modified On:		    NIL
      Purpose:			    This method is used to execute Stored Proc and return results in a DataSet.
      No of Arguments:      1
      Returns:			    DataSet
      Notes:			    NIL
      ***********************************************************/
        public DataSet ExecuteDataSet(string cmdText)
        {
            SqlConnection conn = new SqlConnection();
            conn = GetConnection();
            DataSet ds = new DataSet();

            try
            {
                SqlCommand oCmd = new SqlCommand(cmdText, conn);
                oCmd.CommandType = CommandType.Text;
                //oCmd.CommandTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["SQLCommandTimeout"]);
                oCmd.CommandTimeout = 0;

                DateTime dtStart = DateTime.Now;

                SqlDataAdapter daDataAdapter = new SqlDataAdapter(oCmd);
                daDataAdapter.Fill(ds);

                TimeSpan tsTime = DateTime.Now.Subtract(dtStart);
                logger.writeDBQueryTime(oCmd.CommandText, dtStart.Second);

                return ds;

            }


            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }

        /**********************************************************
       Function:			CloseConn	
       Created By:			Abhilash Rana
       Created On:			23-August-2010
       Modified By:		    NIL
       Modified On:		    NIL
       Purpose:			    This method is used to close SQL connection
       No of Arguments:     1
       Returns:			    NIL
       Notes:			    NIL
       ***********************************************************/
        public void CloseConn(SqlConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /**********************************************************
       Function:			GetConnection	
       Created By:			Abhilash Rana
       Created On:			23-August-2010
       Modified By:		    NIL
       Modified On:		    NIL
       Purpose:			    This method is used establish connection with SQL DB
       No of Arguments:     0
       Returns:			    SqlConnection
       Notes:			    NIL
       ***********************************************************/
        public SqlConnection GetConnection()
        {
            try
            {
                string str;
                str = ConfigurationSettings.AppSettings["SQLConnString"];
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            }
}