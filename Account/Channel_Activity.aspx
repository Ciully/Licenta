<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Channel_Activity.aspx.cs" Inherits="Account_Channel_Activity" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ImageButton ID="ProfilePic" runat="server" data-toggle="modal" data-target="#myModal" />    
    <ul class="nav nav-tabs nav-pills" role="tablist">
        <li role="presentation"><a id="huome" runat="server">Home</a></li>
        <li role="presentation"><a id="vdeos" runat="server">Videos</a></li>
        <li role="presentation"><a id="Pleleste" runat="server">Playlists</a></li>
        <li role="presentation"><a id="abut" runat="server">About</a></li>
        <li role="presentation" class="active"><a id="activete" runat='server'>Activity</a></li>
    </ul>


    <asp:ScriptManagerProxy runat="server"></asp:ScriptManagerProxy>
    <asp:UpdatePanel runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="channel_container" class="form-inline" runat="server">
                <div class="form-group">
                    <input id="Main_input" class="form-control" type="text" placeholder="Your comments" runat="server" />
                </div>
                <div class="form-group">
                    <asp:Button ID="Refresh_Comments" ClientIDMode="Static" Text="Refresh" class="btn btn-default hide" runat="server" OnClick="Refresh_Comments_ServerClick" Style="display: " />
                    <button class="btn btn-default" runat="server" onserverclick="Post_comment">Add</button>
                </div>
            </div>

            <asp:Repeater ID="Wall_posts" runat="server" OnItemDataBound="R2_ItemDataBound">
                <ItemTemplate>
                    <div>
                        <div class="commenterImage">
                            <a href='<%#Eval("User_posted","~/Account/Channel?id={0}") %>' runat="server">
                                <img src='<%#Eval("icon_path",@"{0}") %>' runat="server" />
                            </a>

                        </div>

                        <a href='<%#Eval("User_posted","~/Account/Channel?id={0}") %>' runat="server">
                            <p><%# Eval("UserName") %> </p>
                        </a>
                        <%--                <% if (Eval("User_posted").ToString() == Eval("User_wall").ToString() || Convert.ToString(Eval("User_wall")) == User.Identity.GetUserId())
                   {%>
                <button type='button' class='close' aria-hidden='true' runat='server' onserverclick='delete_post'>&times;</button>
                <%}%>--%>
                        <asp:Button Text="&times;" runat="server" ID="DeleteButton" CssClass='close' aria-hidden='true' CommandName="Delete" CommandArgument='<%#Eval("Id") %>' OnCommand='delete_post' Visible="false" />
                        <div class="commentText">
                            <p class="">
                                <%# Eval("Post_Text") %>
                            </p>
                            <span class="date sub-text">on <%# Eval("date_posted") %></span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
