<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Channel_videos.aspx.cs" Inherits="Account_Channel_videos" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" src="~/Scripts/JavaScript.js" runat="server"></script>


    <asp:Image ID="ProfilePic" runat="server" />
    <ul class="nav nav-tabs nav-pills" role="tablist">
        <li role="presentation"><a id="huome" runat="server">Home</a></li>
        <li role="presentation" class="active"><a id="vdeos" runat="server">Videos</a></li>
        <li role="presentation"><a id="Pleleste" runat="server">Playlists</a></li>
        <li role="presentation"><a id="abut" runat="server">About</a></li>
        <li role="presentation"><a id="activete" runat='server'>Activity</a></li>
    </ul>


    <div id="channel_container" class="form-inline" runat="server">


    <asp:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <div class="modal fade" id="Save_modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                <div class="modal-dialog"></div>
            </div>
            <div class="modal fade" id="aditional_info" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title"><%:Session["vid_title"].ToString() %></h4>
                        </div>
                        <div class="modal-body">
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

                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                            <button type="button" class="btn btn-primary" data-dismiss="modal" onserverclick="Submit_Click" runat="server">Save changes</button>
                        </div>
                    </div>
                    <!-- /.modal-content -->
                </div>
                <!-- /.modal-dialog -->
            </div>
            <!-- /.modal -->
            <div class="input-group">
                <asp:TextBox runat="server" class="form-control" placeholder="Search for a Video" aria-describedby="basic-addon2" ID="Video_stcInput" />
                <button class="input-group-addon" runat="server" onserverclick="Button3_ServerClick" id="Button3">Search</button>
            </div>
            <ul id="VideoList" style="list-style:none">
            <asp:Repeater ID="video_list" runat="server">
                <ItemTemplate>
                    <li>
                    <div class="media">
                        <div class="media-left">
                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                            </a>
                        </div>
                        <div class="media-body">
                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                <h4 class="media-heading"><%#Eval("Title") %></h4>
                            </a>
                            <span class="date sub-text">on <%# Eval("date_posted") %></span>
                            <p style="float: right"><%#Eval("Status") %> </p>
                            <div id='btn_update<%# Eval("Id") %>'>
                                <asp:Button ID="Button1" Text="Update" runat="server" CssClass="btn btn-default" CommandName="Update" OnCommand="CommandBtn_Click" CommandArgument='<%#Eval("Id") + "#" + Eval("Title") + "#" + Eval("Description") + "#" + Eval("Tags")%>' />
                                <asp:Button ID="Button2" Text="Delete" runat="server" CssClass="btn btn-danger close" CommandName="Delete" OnCommand="CommandBtn_Click" CommandArgument='<%#Eval("Id")%>' />                            
                            </div>
                        </div>
                    </div></li>
                </ItemTemplate>
            </asp:Repeater>
                </ul>
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>

    <script>
        function PendingColor() {
            var c = document.getElementById("VideoList");
            var sub = c.getElementsByTagName("li");
            for (i = 0; i < sub.length; i++) {
                sub[i].style.backgroundColor = "white";
            }
            for (i = 0; i < sub.length; i++) {
                if (sub[i].innerHTML.indexOf("Pending") !== -1) {
                    sub[i].style.backgroundColor = "#E7E7E7";
                }

            }
        }
    </script>
</asp:Content>
