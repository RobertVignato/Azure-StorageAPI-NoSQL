using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;

namespace A2DWebRole1
{
    public class BusinessLogic
    {

        internal DataTable Get_FullName(Guid userGUID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = new SqlCommand("sp_GetFullNameForUserGUID", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserGUID", userGUID);
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        internal DataTable Get_Topics(int ForumID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = new SqlCommand("sp_GetForum", cn); //
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ForumID", @ForumID);
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        internal DataTable Get_Topic(string TopicGUID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = new SqlCommand("sp_GetTopic", cn); //
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TopicGUID", @TopicGUID);
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        internal DataTable Get_Forums()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = new SqlCommand("sp_GetForumns", cn); //
            cmd.CommandType = CommandType.StoredProcedure;
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        // For brevity, other CRUD operations have been omitted from this file
    }
}