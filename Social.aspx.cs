using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.WindowsAzure;

namespace A2DWebRole1
{
    public partial class Social : System.Web.UI.Page
    {
        #region --- Accessors ---

        private string _UserName { get; set; }
        private Guid _UserGUID { get; set; }
        private string _StrUserGUID { get; set; }

        private string _LastName { get; set; }
        private string _FirstName { get; set; }
        private string _FullName { get; set; }

        private string _TopicName { get; set; }
        private string _TopicGUID { get; set; }

        private string _SubjectName { get; set; }
        private string _SubjectGUID { get; set; }

        private string _ForumName { get; set; }
        private string _ForumGUID { get; set; }

        //private string _PostText { get; set; }
        private string _PostGUID { get; set; }

        //private Guid _UserGUID = System.Guid.NewGuid();

        //MembershipCreateStatus _membStatus;

        private string _IPAddress { get; set; }

        private string _PostText { get; set; }
        private string _PostText_rm { get; set; }
        private string _PostText_ck { get; set; } // SQL Injection Testing

        private Guid _CommentGUID { get; set; }
        private string _StrCommentGUID { get; set; }

        private DateTime _CreatedOn = DateTime.Now.ToLocalTime();

        private string _CommentorFirstName { get; set; }
        private string _CommentorLastName { get; set; }


        #region - Human User Validation -

        private string strNum1 { get; set; }
        private string strNum2 { get; set; }
        private string _StrCaptchaAnswer { get; set; }
        private string _StrCaptchaAnswer_ck { get; set; }

        private int Num1 { get; set; }
        private int Num2 { get; set; }
        private int _CaptchaAnswer { get; set; }
        private int _CaptchaAnswer_ck { get; set; }

        #endregion

        #endregion

        BusinessLogic bl = new BusinessLogic();
        Utilities utilities = new Utilities();

        protected void Page_Load(object sender, EventArgs e)
        {
            #region --- Check if the user is authenticated ---

            if (!Request.IsAuthenticated)
            {
                tbxComment.Text = "Login to comment";
                tbxComment.Enabled = false;
                btnSubmitComment.Enabled = false;
            }
            else
            {
                MembershipUser myObject = Membership.GetUser();
                _UserGUID = (Guid)myObject.ProviderUserKey;
                _StrUserGUID = _UserGUID.ToString();
                _UserName = myObject.UserName;
            }
            #endregion

            if (!IsPostBack)
            {
                this.Bind_Forums();
                this.GenerateQuestionForHumanAuthentication(); // I need this to refresh after each submission
            }
        }

        private void GenerateQuestionForHumanAuthentication()
        {
            int[] randNumArray = utilities.GetTwoRandomNumbers();
            Num1 = randNumArray[0];
            strNum1 = Num1.ToString();
            Session["Num1"] = strNum1;
            Num2 = randNumArray[1];
            strNum2 = Num2.ToString();
            Session["Num2"] = strNum2;
            string MyCaptchaText = ConfigurationManager.AppSettings["MyCaptchaText"].ToString();
            lblMyCaptcha.Text = String.Format("{0} {1} + {2} ? ", MyCaptchaText, strNum1, strNum2);
        }

        protected void btnSubmitComment_Click(object sender, EventArgs e)
        {
            this.ValidateUserSubmission();
        }

        private void ValidateUserSubmission()
        {
            _IPAddress = GetIpAddress();

            #region - Form Validation -

            bool status0;
            int count = 0;

            #region - tbxComment -

            if (tbxComment.Text == String.Empty)
            {
                tbxComment.CssClass = "textboxYellow";
                lblMessage.Text = "*";
            }
            else
            {
                _PostText = tbxComment.Text;
                _PostText_rm = utilities.StripSpaces(_PostText);

                // Test for SQL Injection Test
                bool Message = utilities.TestStringForSQLInjection(_IPAddress, _PostText_rm);
                if (Message) // SQL Injection Detected
                {
                    _PostText_ck = String.Empty;
                    this.Send_SQLInjection_Notification_Email(_PostText);
                    tbxComment.Text = String.Empty;
                    // Noting else happens
                }
                else // SQL Injection NOT Detected
                {
                    _PostText_ck = _PostText;
                    tbxComment.CssClass = "textbox";
                    count++;
                }
            }
            #endregion

            #region - tbxMyCAPTCHAAnswer -

            if (tbxMyCAPTCHAAnswer.Text == String.Empty)
            {
                tbxMyCAPTCHAAnswer.CssClass = "textboxYellow";
                lblMyCapInfo.Text = "*";
            }
            else
            {
                _StrCaptchaAnswer = tbxMyCAPTCHAAnswer.Text;

                // Test for SQL Injection Test
                bool captchaAnswer = utilities.TestStringForSQLInjection(_IPAddress, _StrCaptchaAnswer);
                if (captchaAnswer) // SQL Injection Detected
                {
                    _StrCaptchaAnswer_ck = String.Empty;
                    this.Send_SQLInjection_Notification_Email(_StrCaptchaAnswer);
                    // Noting else happens
                }
                else // SQL Injection NOT Detected
                {

                    string lower = _StrCaptchaAnswer.ToLower();
                    _StrCaptchaAnswer_ck = utilities.StripSpaces(lower);
                    tbxMyCAPTCHAAnswer.CssClass = "textbox";
                    lblMyCapInfo.Text = String.Empty;

                    bool IsNumber = utilities.TestStringForNumeric(_StrCaptchaAnswer_ck);
                    if (IsNumber)
                    {
                        int num1 = Convert.ToInt16(Session["Num1"]);
                        int num2 = Convert.ToInt16(Session["Num2"]);
                        _CaptchaAnswer = Convert.ToInt16(_StrCaptchaAnswer_ck);
                        bool result = utilities.Solve_AddTwoNumbers(num1, num2, _CaptchaAnswer);
                        if (result)
                        {
                            //lblResults.Text = "<font color=\"Green\">* Correct !</font>";
                            lblResults.Text = String.Empty;
                            count++;
                        }
                        else
                        {
                            lblResults.Text = "<font color=\"Red\">* Sorry... Wrong answer.</font>";
                        }
                    }
                    else
                    {
                        lblMyCapInfo.Text = "Please enter a number";
                    }
                }
            }
            #endregion

            if (count > 1) status0 = true;
            else status0 = false;

            #endregion

            #region - (status0) -

            if (status0) // The Form entries are OK. Go ahead and process
            {

                if (!Request.IsAuthenticated)
                {
                    tbxComment.Text = "Login to comment";
                    tbxComment.Enabled = false;
                    btnSubmitComment.Enabled = false;
                }
                else
                {
                    var statusMessage = String.Empty;
                    try
                    {
                        _CommentGUID = Guid.NewGuid();
                        _StrCommentGUID = _CommentGUID.ToString();
                        _StrUserGUID = _UserGUID.ToString();

                        _TopicName = ViewState["_TopicName"].ToString();
                        _TopicGUID = ViewState["_TopicGUID"].ToString();
                        _ForumName = ViewState["_ForumName"].ToString();
                        _ForumGUID = ViewState["_ForumGUID"].ToString();

                        // Get FullName for Post Identity
                        DataTable dt = bl.Get_FullName(_UserGUID);
                        _FullName = dt.Rows[0][1].ToString();

                        var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                        var context = new ForumPostDataServiceContext(account.TableEndpoint.ToString(), account.Credentials);
                        context.AddForumPost(
                            _CreatedOn
                            , _UserName
                            , _StrUserGUID
                            , _FullName
                            , _TopicName
                            , _TopicGUID
                            , _ForumName
                            , _ForumGUID
                            , Server.HtmlEncode(this.tbxComment.Text)
                            , _StrCommentGUID //_PostGUID
                            );

                        var query = (from entity in context.CreateQuery<ForumPost>("ForumPosts")
                                     where (entity.TopicGUID.Equals(_TopicGUID))
                                     select entity);
                        this.lvComments.DataSource = query;
                        this.lvComments.DataBind();
                    }
                    catch (System.Data.Services.Client.DataServiceRequestException ex)
                    {
                        statusMessage = "Unable to connect to the table service. Please check that the service is running.<br />"
                                         + ex.Message;
                    }
                    finally
                    {
                        tbxComment.Text = String.Empty;
                        lblResults.Text = String.Empty;
                        lblResults.CssClass = "";
                        this.SetFocus(tbxComment);
                    }
                }
            }
            else
            {
                //lblResults.Text += "<br />(status0) else - The form is bad " + DateTime.Now.ToString();
            }
            #endregion
        }

        public string GetIpAddress()
        {
            string strIpAddress;
            strIpAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (strIpAddress == null)
            {
                strIpAddress = Request.ServerVariables["REMOTE_ADDR"];
            }
            else
            {
                strIpAddress = HttpContext.Current.Request.UserHostAddress;
            }
            return strIpAddress;
        }

        private void Send_SQLInjection_Notification_Email(string injectionString)
        {
            Email eMsg = new Email();
            eMsg.To = ConfigurationManager.AppSettings["To_Administrator"].ToString();
            eMsg.From = ConfigurationManager.AppSettings["From_Administrator"].ToString();
            eMsg.Cc = ConfigurationManager.AppSettings["Cc_Administrator"].ToString();
            eMsg.Bcc = ConfigurationManager.AppSettings["Bcc_Administrator"].ToString();
            eMsg.IPAddress = _IPAddress;
            Uri TheWholeUrl = Request.Url;
            eMsg.Url = TheWholeUrl.ToString(); // Get the URL of the Webpage where the attack was attempted.
            eMsg.InjectionString = injectionString;
            //eMsg.SendEmail_ToAdmins("SQLInjectionAttempt! Jumbuh SignUp");
        }



        protected void Bind_Forums()
        {
            DataTable dt = bl.Get_Forums();
            lvForums.DataSource = dt;
            lvForums.DataBind();
            phComments.Visible = false;
            tbxComment.Enabled = false;
        }



        protected void lvForums_ItemCommand(object sender, System.Web.UI.WebControls.ListViewCommandEventArgs e)
        {
            lblResults.Text = String.Empty;

            if (e.CommandName == "BindTopics")
            {
                string selectedValue = e.CommandArgument.ToString();
                int ForumID = (int)Convert.ToInt32(selectedValue);
                this.Bind_Topics(ForumID);
                phComments.Visible = false;
            }
        }



        protected void Bind_Topics(int ForumID)
        {
            DataTable dt = bl.Get_Topics(ForumID);
            lvTopics.DataSource = dt;
            lvTopics.DataBind();
            lblBreadCrumb_Forum.Text = " / " + dt.Rows[0][2].ToString(); //.ToUpper();

            _ForumGUID = dt.Rows[0][1].ToString(); ViewState["_ForumGUID"] = _ForumGUID;
            _ForumName = dt.Rows[0][2].ToString(); ViewState["_ForumName"] = _ForumName;
            lblBreadCrumb_Forum.Text = " / " + ViewState["_ForumName"]; //.ToUpper();
            phComments.Visible = false;
            tbxComment.Enabled = false;
        }



        protected void lvTopics_ItemCommand(object sender, System.Web.UI.WebControls.ListViewCommandEventArgs e)
        {
            lblResults.Text = String.Empty;

            if (e.CommandName == "BindPosts")
            {
                string TopicGUID = e.CommandArgument.ToString();
                this.Bind_Posts(TopicGUID);

                if (!Request.IsAuthenticated)
                {
                    tbxComment.Text = "Login to comment";
                    tbxComment.Enabled = false;
                    btnSubmitComment.Enabled = false;
                }
                else
                {
                    tbxMyCAPTCHAAnswer.Enabled = true;
                    tbxComment.Enabled = true;
                    btnSubmitComment.Enabled = true;
                }
            }
        }



        protected void Bind_Posts(string TopicGUID)
        {
            phComments.Visible = true;
            tbxComment.Enabled = true;

            var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            var context = new ForumPostDataServiceContext(account.TableEndpoint.ToString(), account.Credentials);

            var query = (from entity in context.CreateQuery<ForumPost>("ForumPosts")
                            where (entity.TopicGUID.Equals(TopicGUID))
                            select entity);
            this.lvComments.DataSource = query;
            this.lvComments.DataBind();

            DataTable dt = bl.Get_Topic(TopicGUID);
            _TopicGUID = dt.Rows[0][1].ToString(); ViewState["_TopicGUID"] = _TopicGUID;
            _TopicName = dt.Rows[0][2].ToString(); ViewState["_TopicName"] = _TopicName;            

            lblBreadCrumb_Forum.Text = " / " + ViewState["_ForumName"] + " / " + ViewState["_TopicName"]; //.ToUpper();
        }



        protected void lvComments_ItemCommand(object sender, System.Web.UI.WebControls.ListViewCommandEventArgs e)
        {
            if (e.CommandName == "RemovePost")
            {
                string selectedValue = e.CommandArgument.ToString();
                var account = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
                var context = new ForumPostDataServiceContext(account.TableEndpoint.ToString(), account.Credentials);
                try
                {
                    var query = (from entity in context.CreateQuery<ForumPost>("ForumPosts")
                                 where (entity.UserGUID.Equals(_StrUserGUID) && entity.PostGUID.Equals(selectedValue))
                                 select entity).Single();

                    context.DeleteObject(query);
                    context.SaveChanges();
                    
                    _TopicGUID = ViewState["_TopicGUID"].ToString();

                    var query2 = (from entity in context.CreateQuery<ForumPost>("ForumPosts")
                                  where (entity.TopicGUID.Equals(_TopicGUID))
                                  select entity);

                    this.lvComments.DataSource = query2;
                    this.lvComments.DataBind();

                    lblResults.Text = String.Empty;
                }
                catch (Exception ex)
                {
                    //lblResults.Text = ex.ToString();
                    lblResults.Text = "<br />You can't delete this post...<br />";
                    lblResults.CssClass = "Red";
                }
            }
        }

    }
}