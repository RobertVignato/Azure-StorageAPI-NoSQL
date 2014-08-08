<%@ Page Title="Artist2Dealer - Socialize" Language="C#" MasterPageFile="~/Site2.Master" AutoEventWireup="true" CodeBehind="Social.aspx.cs" Inherits="A2DWebRole1.Social" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
<!--
    function clearDefault(el) {
        if (el.defaultValue == el.value) el.value = ""
    }
    // -->
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    
    <div class="breadCrumb">
        <span class="BonW">&nbsp;/&nbsp;SOCIALIZE
        <asp:Label ID="lblBreadCrumb_Forum" runat="server" />
        <asp:Label ID="lblBreadCrumb_Topic" runat="server" />
        </span>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div align="center">
        
    <table style="width: 90%; text-align:center; border-spacing: 20px;">
        <tr>
            <td style="vertical-align: top; width: 175px;">
                <span class="categorySection">Step 1<br />
                Select a Forum</span>
            </td>
            
            <td rowspan="3" style="width: 10px;" class="rightDividerDots"></td>
            
            <td style="vertical-align: top; width: 175px;">
                <span class="categorySection">Step 2<br />
                Select a Topic</span></td>
            
            <td rowspan="3" style="width: 10px;" class="rightDividerDots"></td>
            
            <td style="vertical-align: top;">
                <span class="categorySection">Step 3<br />
                Post a Message (300 chars)</span></td>            
        </tr>
        <tr>
            <td style="vertical-align: top; text-align:left; line-height: 1.2em;" class="socialize">

                <asp:ListView ID="lvForums" runat="server" OnItemCommand="lvForums_ItemCommand" DataKeyNames="ForumID">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnForums" runat="server" CommandName="BindTopics" CommandArgument='<%# Eval("ForumID") %>'  Text='<%# Eval("ForumName") %>' /><br />
                    </ItemTemplate>
                </asp:ListView>

            </td>
            <td style="vertical-align: top; text-align:left; line-height: 1.2em;" class="socialize">
                

                <asp:ListView ID="lvTopics" runat="server" OnItemCommand="lvTopics_ItemCommand"  DataKeyNames="TopicGUID">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnTopics" runat="server"  CommandName="BindPosts" CommandArgument='<%# Eval("TopicGUID") %>'  Text='<%# Eval("TopicName") %>' /><br />
                    </ItemTemplate>
                </asp:ListView>


            </td>
            <td style="vertical-align: top; text-align:left; border: solid 0px;">
                
                <div align="center">

                <asp:TextBox ID="tbxComment" runat="server" Width="100%" Height="50px"
                    CssClass="textbox" onfocus="clearDefault(this)" 
                    TextMode="MultiLine" onkeyup="return validateLimit(this, 'lblCount', 300)" Enabled="False"  />
                <br />
                <asp:Label ID="lblMessage" runat="server" CssClass="red10"/>
                    <div id="lblCount" class="Red"></div>

                <br />

                <span class="mycaptcha">

                <asp:Label ID="lblMyCaptcha" runat="server" />
                &nbsp;
                <asp:Label ID="lblMyCapInfo" runat="server" CssClass="red" />

                </span>

                <asp:TextBox ID="tbxMyCAPTCHAAnswer" CssClass="textbox" runat="server" Width="20px" Enabled="False"   />

                <br />
                <br />

                <asp:Button ID="btnSubmitComment" runat="server" Text="Submit" onclick="btnSubmitComment_Click" CssClass="buttonSM" Enabled="False"  />
                <br />
                <asp:Label ID="lblResults" runat="server" CssClass="signUpResults" />

                </div>

                <br />

                <asp:PlaceHolder ID="phComments" runat="server">

                    <asp:ListView ID="lvComments" runat="server" 
                            onitemcommand="lvComments_ItemCommand" DataKeyNames="PostGUID">

                        <ItemTemplate>

                            <table style="width: 100%;" class="comment">
                                <tr>
                                    <td style="vertical-align: top;">

                                        <div>
                                            <span class="commentUsername">
                                                <%# Eval("FullName")%>
                                            </span>-
                                            <span class="commentCreatedOn">
                                                <%# Eval("CreatedOn")%> (GMT)
                                            </span>
                                        </div>
                                        <div class="commentBody">
                                            <%# Eval("PostText")%>
                                        </div>
                                        <div align="right" class="remove">
                                            <asp:LinkButton ID="linkBtnRemovePost" runat="server" Text="remove" CausesValidation="False" 
                                                CommandName="RemovePost" CommandArgument='<%# Eval("PostGUID") %>' />                
                                        </div>

                                    </td>
                                </tr>
                            </table>

                        </ItemTemplate>

                    </asp:ListView>

                </asp:PlaceHolder>

            </td>
        </tr>
    </table>

        
    

    <div id="fb-root"></div>

    <script>
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/all.js#xfbml=1";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));
    </script>

    <br />

    <div class="fb-like-box" data-href="http://www.facebook.com/artist2dealer" 
        data-width="700" data-show-faces="true" data-stream="false" data-header="false">
    </div>



    
    </div>

    <script src="Scripts/validateLimit.js"></script>

</asp:Content>
