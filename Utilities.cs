using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace A2DWebRole1
{
    public class Utilities
    {
        Email em = new Email();

        #region - Accessors - 
        
        private int _MatchID { get; set; }
        private int _D_ID { get; set; }
        private Guid _D_UserGUID { get; set; }
        private string _D_Name { get; set; }
        private string _D_Email { get; set; }
        private int _A_ID { get; set; }
        private Guid _A_GUID { get; set; }
        private string _A_LastName { get; set; }
        private string _A_FirstName { get; set; }
        private string _A_City { get; set; }
        private string _A_Country { get; set; }
        private Guid _A_ProfileArtworkGUIDSmall { get; set; }
        private string _A_Email { get; set; }

        #endregion

        internal bool Send_Match_Notifications()
        {
            bool status = false;

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = new SqlCommand("sp_Get_EmailAddressesForMatches", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);

            try
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    _MatchID = Convert.ToInt32(table.Rows[i][0]);
                    _D_ID = Convert.ToInt32(table.Rows[i][1]);
                    _D_UserGUID = new Guid(table.Rows[i][2].ToString());
                    _D_Name = table.Rows[i][3].ToString();
                    _D_Email = table.Rows[i][4].ToString();
                    _A_ID = Convert.ToInt32(table.Rows[i][5]);
                    _A_GUID = new Guid(table.Rows[i][6].ToString());
                    _A_LastName = table.Rows[i][7].ToString();
                    _A_FirstName = table.Rows[i][8].ToString();
                    _A_City = table.Rows[i][9].ToString();
                    _A_Country = table.Rows[i][10].ToString();
                    _A_ProfileArtworkGUIDSmall = new Guid(table.Rows[i][11].ToString());
                    _A_Email = table.Rows[i][12].ToString();

                    em.SendGrid_ToDealer_ReArtistMatches(
                           _D_ID
                           , _D_UserGUID
                           , _D_Name
                           , _D_Email
                           , _A_ID
                           , _A_GUID
                           , _A_LastName
                           , _A_FirstName
                           , _A_City
                           , _A_Country
                           , _A_ProfileArtworkGUIDSmall
                           , _A_Email
                           );

                    em.SendGrid_ToArtist_ReDealerMatches(
                           _D_ID
                           , _D_UserGUID
                           , _D_Name
                           , _D_Email
                           , _A_ID
                           , _A_GUID
                           , _A_LastName
                           , _A_FirstName
                           , _A_City
                           , _A_Country
                           , _A_ProfileArtworkGUIDSmall
                           , _A_Email
                           );

                    this.Update_QNA_Matches_IsNotifed(_MatchID);
                }
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }

        internal DataTable Get_Matches_Unnotified_Count()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT " +
                "COUNT(*) " +
                "FROM QNA_Matches " +
                "WHERE IsNotified = 0";
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        internal DataTable Get_Matches_Unnotified()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT " +
                "[MatchID] " +
                ",[DealerID] " +
                ",[ArtistID] " +
                ",[CreatedOn] " +
                //",[IsNotified] " +
                "FROM QNA_Matches " +
                "WHERE IsNotified = 0 " +
                "ORDER BY MatchID DESC";
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        internal DataTable Get_ChronJobs()
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT " +
                "[ChronJobID] " +
                ",[JobName] " +
                ",[JobDescription] " +
                ",[StoredProcedure] " +
                ",[CreatedOn] " +
                ",[UserGUID] " +
                "FROM ChronJobs " +
                "ORDER BY ChronJobID DESC";
            DataTable table = DataAccess.ExecuteSelectCommand(cmd);
            return table;
        }

        internal bool Create_ChronJob(
            string JobName
            , string JobDescription
            , string StoredProcedure
            , Guid UserGUID
            )
        {
            bool status;

            #region --- SQL Command ---

            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "INSERT INTO ChronJobs " +
                                    "(" +
                                       "JobName" +
                                       ",JobDescription" +
                                       ",StoredProcedure" +
                                       ",UserGUID" +
                                    ") " +
                                "VALUES " +
                                    "(" +
                                       "@JobName " +
                                       ",@JobDescription " +
                                       ",@StoredProcedure " +
                                       ",@UserGUID " +
                                    ")";
            #endregion

            #region --- Parameters ---

            cmd.Parameters.AddWithValue("@JobName", JobName);
            cmd.Parameters.AddWithValue("@JobDescription", JobDescription);
            cmd.Parameters.AddWithValue("@StoredProcedure", StoredProcedure);
            cmd.Parameters.AddWithValue("@UserGUID", UserGUID);

            #endregion

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                status = false;
            }
            finally
            {
                cn.Close();
            }
            return status;
        }

        internal bool Run_sp_Matching_Initializer()
        {
            bool status;
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = new SqlCommand("sp_Matching_Initializer", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                status = false;
            }
            finally
            {
                cn.Close();
            }
            return status;
        }

        internal bool Create_ChronJob_Record(string jobName, string jobDescription, Guid userGUID)
        {
            bool status;
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "INSERT INTO ChronJobs " +
                                    "(" +
                                       "JobName " +
                                       ",JobDescription " +
                                       ",UserGUID " +
                                    ") " +
                                "VALUES " +
                                    "(" +
                                       "@JobName " +
                                       ",@JobDescription" +
                                       ",@UserGUID " +
                                    ")";

            #region --- Parameters ---

            cmd.Parameters.AddWithValue("@UserGUID", userGUID);
            cmd.Parameters.AddWithValue("@JobName", jobName);
            cmd.Parameters.AddWithValue("@JobDescription", jobDescription);

            #endregion

            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                status = false;
            }
            finally
            {
                cn.Close();
            }
            return status;
        }

        #region - MyCAPTCHA - 
        
        internal int GetRandomNumber()
        {
            Random random = new Random();
            int x = random.Next(10);
            return x;
        }

        internal int[] GetTwoRandomNumbers()
        {
            Random random = new Random();
            int[] x = new int[2];
            x[0] = random.Next(10);
            x[1] = random.Next(10);
            return x;
        }

        internal bool Solve_AddTwoNumbers(int num1, int num2, int userSubmission)
        {
            int sum = num1 + num2;
            bool result = userSubmission == sum ? true : false;
            return result;
        }

        #endregion

        internal bool TestForValidEmailAddress(string emailAddress)
        {
            string pattern = @"[\w*.]+@[\w.]+\.\w+";
            bool test = Regex.IsMatch(emailAddress, pattern);
            return test;
        }

        internal bool TestStringForNumeric(string input)
        {
            string pattern = @"\d";
            bool test = Regex.IsMatch(input, pattern);
            return test;
        }

        internal bool TestStringForYear(string input)
        {
            string pattern = @"(19|20)\d{2}";
            bool test = Regex.IsMatch(input, pattern);
            return test;
        }

        internal bool TestStringForSQLInjection(string ip, string nastyCode)
        {
            string pattern = ConfigurationManager.AppSettings["REGEX_Injection"].ToString();
            bool test = Regex.IsMatch(nastyCode, pattern);
            if (test) this.LogSQLInjectionAttempt(ip, nastyCode);
            return test;
        }

        internal bool TestStringForSQLInjection_HTTP(string ip, string nastyCode)
        {
            string pattern = ConfigurationManager.AppSettings["REGEX_Injection_HTTP://"].ToString();
            bool test = Regex.IsMatch(nastyCode, pattern);
            if (test) this.LogSQLInjectionAttempt(ip, nastyCode);
            return test;
        }

        internal bool TestForMalicious(string nastyCode)
        {
            string pattern = ConfigurationManager.AppSettings["REGEX_Injection"].ToString();
            bool test = Regex.IsMatch(nastyCode, pattern);
            //if (test) this.LogSQLInjectionAttempt(ip, nastyCode);
            return test;
        }

        internal bool LogSQLInjectionAttempt(string IPAddress, string NastyCode)
        {
            bool status;
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "INSERT INTO SQLInjections " +
                                    "(" +
                                        "IPAddress" +
                                        ", NastyCode" +
                                    ") " +
                                "VALUES " +
                                    "(" +
                                        "@IPAddress" +
                                        ", @NastyCode" +
                                    ")";
            cmd.Parameters.AddWithValue("@IPAddress", IPAddress);
            cmd.Parameters.AddWithValue("@NastyCode", NastyCode);
            try
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
                status = false;
            }
            finally
            {
                cn.Close();
            }
            return status;
        }

        internal bool IsValid_PageVisitor(Guid userGUID, Guid identityGUID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();

            #region - SELECT -
            cmd.CommandText = "SELECT " +
                "COUNT(IdentityGUID) " +
                "FROM Identities " +
                "WHERE (UserGUID = @UserGUID) AND (IdentityGUID = @IdentityGUID)";
            cmd.Parameters.AddWithValue("@UserGUID", userGUID);
            cmd.Parameters.AddWithValue("@IdentityGUID", identityGUID);
            #endregion

            cn.Open();

            bool status = false;
            int count = (int)cmd.ExecuteScalar();
            cn.Close();
            if (count > 0) status = true;
            else status = false;
            return status;
        }

        internal bool IsLast_InterestUser(string strInterestGUID)
        {
            SqlConnection cn = new SqlConnection();
            cn.ConnectionString = AppConfig.DbConnectionString;
            SqlCommand cmd = cn.CreateCommand();

            cmd.CommandText = "SELECT " +
                "COUNT(InterestGUID) " +
                "FROM IdentitiesInInterests " +
                "WHERE (InterestGUID = @InterestGUID)";
            cmd.Parameters.AddWithValue("@InterestGUID", strInterestGUID);

            cn.Open();

            bool status = false;
            int count = (int)cmd.ExecuteScalar();
            cn.Close();
            if (count > 1) status = true;
            else status = false;
            return status;
        }

        Random random = new Random();

        internal string GenerateIdentity()
        {
            string letters;

            int num = random.Next(10000);

            StringBuilder randomText = new StringBuilder();
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // abcdefghijklmnopqrstuvwxyz

            for (int j = 0; j <= 3; j++)
            {
                randomText.Append(alphabets[random.Next(alphabets.Length)]);
            }
            letters = randomText.ToString();

            return letters + num;
        }

        //internal int GetRandomNumber()
        //{
        //    Random random = new Random();
        //    int num = random.Next(1000000);
        //    return num;
        //}

        internal void Sleep()
        {
            TimeSpan waitTime = new TimeSpan(0, 0, 1); // 1 second
            Thread.Sleep(waitTime);
        }

        public string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        internal string CleanTextEntries(string strIn)
        {
            string regexfilter = ConfigurationManager.AppSettings["REGEX_Injection"].ToString();
            //string str = this.RemoveAccent(strIn).ToLower();

            return Regex.Replace(strIn, regexfilter, "...");
        }

        internal string CleanPhoneNumber(string strIn)
        {
            return Regex.Replace(strIn, @"\D", "");
        }

        internal string StripDashes(string strIn)
        {
            return Regex.Replace(strIn, @"--", "&&&");
        }

        internal string StripSpaces(string strIn)
        {
            return Regex.Replace(strIn, @" ", "");
        }

        internal string RegEx_LettersOnly(string strIn)
        {
            return Regex.Replace(strIn, @"[^A-Za-z]", " ");
        }

        public int GetNextImageID()
        {
            string sql = "SELECT MAX(ImageID) FROM Images";
            string connectionString = AppConfig.DbConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            conn.Open();
            int retIntVal = (int)cmd.ExecuteScalar();
            conn.Close();
            //return retIntVal + 1;
            return retIntVal;
        }

        public int GetNextDocumentID()
        {
            string sql = "SELECT MAX(DocumentID) FROM Documents";
            string connectionString = AppConfig.DbConnectionString;
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);

            conn.Open();
            int retIntVal = (int)cmd.ExecuteScalar();
            conn.Close();
            //return retIntVal + 1;
            return retIntVal;
        }
    }
}