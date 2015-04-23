<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.Master"  CodeFile="Channel_lists.aspx.cs" Inherits="Account_Channel_lists" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        
    <asp:Image ID="ProfilePic" runat="server"  />
    <ul class="nav nav-tabs nav-pills" role="tablist">
        <li role="presentation"><a id="huome" runat="server">Home</a></li>
        <li role="presentation"><a id="vdeos" runat="server">Videos</a></li>
        <li role="presentation" class="active"><a id="Pleleste" runat="server">Playlists</a></li>
        <li role="presentation"><a id="abut" runat="server">About</a></li>
        <li role="presentation"><a id="activete" runat='server'>Activity</a></li>
    </ul>

    <asp:Repeater ID="list_container" runat="server">
        <ItemTemplate>
            <div class="row">
                <div class="col-sm-6 col-md-4">
                    <div class="thumbnail">
                        <a href='<%#Eval("Id","~/Feed/Playlist?id={0}") %>' runat="server">
                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                            <div>
                                <b><%#Eval("Title") %></b>
                            </div>
                        </a>
                        <span class="date sub-text">on <%# Eval("date_posted") %></span>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>