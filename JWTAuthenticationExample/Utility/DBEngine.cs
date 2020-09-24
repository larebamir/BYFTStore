using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using JWTAuthenticationExample.Models;
using Microsoft.AspNetCore.Authorization;
using JWTAuthenticationExample.Utility;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthenticationExample.Repository
{
    public class DBEngine 
    {
        private static int result = 0;
        private static string connectionString = JWTAuthenticationExample.Startup.DbConnection;
        public static AppDb Db ;
        public DBEngine(AppDb db)
        {
            Db = db;
        }

        public static bool ExecuteNonQuery(MySqlCommand cmd)
        {
            //SqlConnection conn = new SqlConnection(connectionString);
            MySqlConnection conn = Db.GetConnection();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    Db.Connection.Open();

                cmd.Connection = conn;
                result = cmd.ExecuteNonQuery();
                if (result == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Dispose();
            }

        }

        public static string ExecuteScaler(MySqlCommand cmd)
        {

            MySqlConnection conn = Db.GetConnection();
            //SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                string value = null;
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.Connection = conn;
                var firstColumn = cmd.ExecuteScalar();

                if (firstColumn != null)
                {
                    value = firstColumn.ToString();
                }
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Dispose();
            }

        }

        public static DataSet GetDataSet(MySqlCommand cmd)
        {
            MySqlConnection conn = Db.GetConnection();
            //SqlConnection conn = new SqlConnection(connectionString);
            DataSet ds = new DataSet();
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                cmd.Connection = conn;
                MySqlDataAdapter adpt = new MySqlDataAdapter(cmd);
                adpt.Fill(ds);
                adpt.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Dispose();
            }

        }

        public static DataTable GetDataTable(MySqlCommand cmd)
        {
            DataTable dt = new DataTable();
            dt = GetDataSet(cmd).Tables[0];
            return dt;
        }

    }
}