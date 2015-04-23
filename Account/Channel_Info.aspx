<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeFile="Channel_Info.aspx.cs" Inherits="Account_Channel_about" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

        <asp:Image ID="ProfilePic" runat="server"  />
    <ul class="nav nav-tabs nav-pills" role="tablist">
        <li role="presentation"><a id="huome" runat="server">Home</a></li>
        <li role="presentation"><a id="vdeos" runat="server">Videos</a></li>
        <li role="presentation"><a id="Pleleste" runat="server">Playlists</a></li>
        <li role="presentation" class="active"><a id="abut" runat="server">About</a></li>
        <li role="presentation" ><a id="activete" runat='server'>Activity</a></li>
    </ul>


    <div class="form-group" runat="server" id="Update_form" visible="false">
        <label for="message_text" class="control-label">Message:</label>
        <textarea class="form-control" id="message_text" runat="server"></textarea>
        <asp:Button ID="Button3" class="btn btn-default" runat="server" Text="Save Changes" OnClick="Button2_Click" />
        <asp:Button ID="Button2" class="btn btn-default" runat="server" Text="Cancel" OnClick="Button2_Click1" />
    </div>
    <div class="comtainer" runat="server" id="channel_descr_container" visible="true">
    <asp:Button ID="Button1" runat="server" Text="Update" OnClick="Button1_Click" />
        <article runat="server" id="channel_descr">
        </article>

            <span class="date sub-text" id="date_join" runat="server">Joined on </span>
            <asp:Label ID="Label1" runat="server" Text=""></asp:Label>

    </div>
</asp:Content>