<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Upload.aspx.cs" Inherits="Upload" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link id="link1" rel="stylesheet" href="Content/myStyleSheet.css" type="text/css" runat="server" />
    <br />
    <br />
    <div id="vid_up_containre" runat="server">
        <asp:FileUpload ID="VideoUpload" runat="server" /><br />
        <asp:RequiredFieldValidator runat="server" ControlToValidate="VideoUpload"
            CssClass="text-danger" ErrorMessage="An upload link is required." /><br />
        <asp:Button Text="Upload" OnClick="Upload_Click" runat="server" />
    </div>

    <div id="ConversionWraper" class="progress" runat="server" visible="false">
        <asp:Panel runat="server" id="ProgressBar" ClientIDMode="Static" class="progress-bar" role="progressbar" aria-valuenow="70" aria-valuemin="0" aria-valuemax="100" style="width:0%">
        </asp:Panel>
            <asp:Label Text="0%" ID="ProgressText" ClientIDMode="Static" runat="server" />

    </div>

    <div runat="server" id="aditional_info" visible="false">
        <asp:Label ID="VidTitle_labe" runat="server" Text="Give your video a title."></asp:Label><br />
        <asp:TextBox ID="VidTitle" runat="server" CssClass="textBoxes" Height="35px" Width="634px"></asp:TextBox><br />

        <asp:Label ID="VidDescr_label" runat="server" Text="Add a description."></asp:Label><br />
        <asp:TextBox ID="VideoDescription" runat="server" CssClass="textBoxes" Height="128px" Width="631px"></asp:TextBox><br />

        <asp:Label ID="VidTags" runat="server" Text="Add a description."></asp:Label><br />
        <asp:TextBox ID="Tags" runat="server" CssClass="textBoxes" Width="634px"></asp:TextBox><br />

        <asp:Label ID="VidPrivacy" runat="server" Text="Set video privacy."></asp:Label><br />
        <asp:DropDownList ID="VideoPrivacy" runat="server">
            <asp:ListItem>Private</asp:ListItem>
            <asp:ListItem>Public</asp:ListItem>
            <asp:ListItem>Unlisted</asp:ListItem>
        </asp:DropDownList><br />
        <br />

        <asp:Label ID="Label1" runat="server" Text="Select a suitable category."></asp:Label><br />
        <asp:DropDownList ID="CategoryDrop" runat="server"></asp:DropDownList>

        <asp:Label Text="Add a Thumbnail" runat="server" />
        <asp:FileUpload ID="ThumbnailUp" runat="server" /><br />

        <asp:Button runat="server" Text="Submit Video" OnClick="Submit_Click" />
    </div>
</asp:Content>
