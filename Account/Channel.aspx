<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Channel.aspx.cs" Inherits="Account_Channel" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="ModalVIz" runat="server">
        <div id="myModal" class="modal fade bs-example-modal-sm" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-sm">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">Change your Profile Picture</h4>
                    </div>
                    <div class="modal-body">
                        <asp:FileUpload ID="PicUpload" runat="server" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="PicUpload"
                            CssClass="text-danger" ErrorMessage="An upload link is required." /><br />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                        <asp:Button Text="Upload Image" type="button" class="btn btn-primary" OnClick="btnSubmitImage_Click" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <asp:ImageButton ID="ProfilePic" runat="server" data-toggle="modal" data-target="#myModal" />


    <ul class="nav nav-tabs nav-pills" role="tablist">
        <li role="presentation" class="active"><a id="huome" runat="server">Home</a></li>
        <li role="presentation"><a id="vdeos" runat="server">Videos</a></li>
        <li role="presentation"><a id="Pleleste" runat="server">Playlists</a></li>
        <li role="presentation"><a id="abut" runat="server">About</a></li>
        <li role="presentation"><a id="activete" runat='server'>Activity</a></li>
    </ul>



    <div class="panel container">
        <div class="row container">
            <h2>Spotlight</h2>
            <div class="col-md-5">
                <a id="Main_spot_link" href="#" runat="server">
                    <img id="Main_spot_thumb" src="..." alt="..." runat="server" />
                    <h4 class="media-heading" id="Spot_title" runat="server"></h4>
                </a>
                <div class="media-body">
                    <span class="date sub-text">
                        <a id="Spot1UserLink" runat="server">
                            <asp:Label ID="Spot1User" Text="text" runat="server" />
                        </a>
                        on
                        <asp:Label ID="Spot1Date" Text="text" runat="server" />
                        <b>
                            <asp:Label ID="Spor1Views" Text="text" runat="server" />
                            Views</b> </span>
                </div>
            </div>
            <div class="col-md-7">
                <ul class="list-group" style="list-style: none">

                    <asp:Repeater runat="server" ID="SpotLight_Repeater">
                        <ItemTemplate>
                            <li>
                                <div class="media">
                                    <div class="media-left media-middle">
                                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                                        </a>
                                    </div>
                                    <div class="media-body">
                                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                                        </a>
                                        <a href='<%#Eval("UserId","~/Account/Channel?id={0}") %>' runat="server">
                                            <span class="date sub-text">by <%# Eval("UserName") %></span>
                                        </a>
                                        <br />
                                        <span class="date sub-text">on <%# Eval("date_posted") %> <b><%#Eval("Views") %> Views</b> </span>
                                    </div>
                                </div>
                                <hr />
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
    </div>


    <div id="MostRecent">
        <h4>Recently Uploaded</h4>
        <ul class="list-inline">
            <asp:Repeater ID="VIdPlayRep" runat="server">
                <ItemTemplate>
                    <li>
                        <div class="media">
                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                <div class="media-left media-middle">
                                    <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                                </div>
                                <div class="media-body">
                                    <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                        <h4 class="media-heading"><%#Eval("Title") %></h4>
                                    </a>
                                    <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                </div>
                            </a>
                        </div>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>

    <div id="RecentActiv">
        <h4>Recent Activity</h4>
        <ul class="list-group" style="list-style: none">
            <asp:Repeater ID="Activity_Repeater" runat="server">
                <ItemTemplate>
                    <li>
                        <hr />
                        <p><a href="<%#Eval("SubId","~/Account/Channel?id={0}")%>"><b><%#Eval("SubName") %></b></a><%#Eval("InterName") %><span class="date sub-text">on <%#Eval("InterDate")%></span></p>
                        <div class="media">
                            <div class="media-left media-middle">
                                <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                    <img class="media-object" src='<%#Eval("Thumbnail","{0}") %>' runat="server" alt="No Image" />
                                </a>
                            </div>
                            <div class="media-body">
                                <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                    <h4 class="media-heading"><%#Eval("Title") %></h4>
                                </a>
                                <a href='<%#Eval("UserId","~/watch?id={0}") %>' runat="server">
                                    <span class="date sub-text">by <%# Eval("UserName") %></span>
                                </a>
                                <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                <div style="height: 60px; overflow: hidden;"><%#Eval("Description") %></div>
                            </div>
                        </div>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>
    <%--<form id="arid" runat="server"></form>--%>
    <div id="channel_container" runat="server">
        <input id="Main_input" runat="server" />
    </div>

    <script>
        function browse() {
            document.getElementById('<%= PicUpload.ClientID %>').click();
        }
        // var fileup = document.getElementById("MainContent_PicUpload");
        //fileup.addEventListener('change', function () {

        function UploadFileNow() {

            alert("asdfa");

        };

        //$("MainContent_PicUpload").on('change', function () {
        //    alert('shdjgh');
        //    // document.getElementById("MainContent_FileUpload1").click();

        //});
        //function chooseFile() {
        //    document.getElementById("FileUpload1").click();
        //}
    </script>
</asp:Content>
