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
        internal DataTable Get_Topics(int ForumID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "SELECT " +
                "Forums.ForumID " + // 0
                ", Forums.ForumGUID " + // 1
                ", Forums.ForumName " + // 2
                ", Topics.TopicID " + // 3
                ", Topics.TopicGUID " + // 4 
                ", Topics.TopicName " + // 5
                "FROM Forums  " +
                "INNER JOIN Topics ON Forums.ForumID = Topics.ForumID " +
                "WHERE (Topics.IsActive = 1)  " +
                "AND (Topics.IsVisible = 1)  " +
                "AND (Forums.IsActive = 1)  " +
                "AND (Forums.IsVisible = 1)  " +
                "AND (Forums.ForumID = @ForumID) " +
                "ORDER BY Topics.TopicName";

            cmd.Parameters.AddWithValue("@ForumID", ForumID);
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        // For brevity, other CRUD operations have been omitted from this file
    }
}