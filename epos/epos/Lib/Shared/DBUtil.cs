using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

/// <summary>
/// Summary description for DBUtil
/// </summary>
namespace epos.Lib.Shared
{
    public static class DBUtil
    {

        public static DataTable ExecuteDataTable(string cmdText)
        {
            DataTable dt = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["EPConnectionString"].ConnectionString;
            SqlDataAdapter da = new SqlDataAdapter(cmdText, connectionString);
            da.Fill(dt);
            da.Dispose();

            return dt;
        }

        public static SqlDataReader ExecuteReader(string cmdText)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EPConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(cmdText, conn);

            SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return dr;
        }

        public static void Execute(string cmdText)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["EPConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public static object ExecuteScalar(string cmdText)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["EPConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            try
            {
                object value = cmd.ExecuteScalar();
                return value;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public static string EscapeSingleQuote(object str)
        {
            return str.ToString().Replace("'", "''").Trim();
        }
    }
}