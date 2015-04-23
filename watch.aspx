<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="watch.aspx.cs" Inherits="watch" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link id="link1" rel="stylesheet" href="~/Content/myStyleSheet.css" type="text/css" runat="server" />
    <div id="video_container">
        <video runat="server" id="video_player" class="video-js vjs-default-skin" width="100%" height="100%" poster="MY_VIDEO_POSTER.jpg" data-setup="{}" autoplay controls>
            <source id="Video_Player_Source" type="video/mp4" runat="server" />
        </video>
    </div>

    <div id="Playlist_Contet" runat="server" visible="false">
        <ul class="list-inline photo-grid" style="white-space: nowrap; overflow-x: scroll">
            <asp:Repeater ID="PlaylistVidRepeater" runat="server">
                <ItemTemplate>
                    <li>
                        <div class="media">
                            <a href='<%#Eval("Id","~/watch?id={0}&index="+Eval("Index")+"&list="+Request.Params["list"].ToString()) %>' runat="server">
                                <div class="media-left">
                                    <figure>
                                        <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                                    </figure>
                                    <figcaption>
                                        <p>
                                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                                            <span class="date sub-text">by <%# Eval("UserName") %></span>
                                        </p>
                                    </figcaption>
                                </div>
                            </a>
                        </div>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
        </ul>
    </div>

    <div id="video_details">

        <h3>
            <asp:Label ID="Video_title" runat="server" Text="Video Title"></asp:Label></h3>
        <div class="col-md-1">
            <img id="ImageButton1" runat="server" src="" alt="No image" style="width: 65px" />
        </div>
        <div class="col-md-10">
            <a id="HyperLink1" runat="server" href="#"></a>
            <br />
            <asp:Button Text="Subscribe" ID="Sub_Button" CssClass="btn btn-primary " OnClick="Subscription_Button_Click" runat="server" />
        </div>
        <asp:Label ID="View_count" runat="server" Text="0"></asp:Label>

        <br />
        <br />
        <hr />
        <asp:LoginView runat="server" ViewStateMode="Disabled">
            <LoggedInTemplate>
                <asp:Button ID="Add_to_playlist" runat="server" Text="+ Add To" class="btn btn-default unselectable" data-toggle="collapse" data-target="#AddToDropDown" OnClientClick="return false;" />
                <asp:Button Text="Fav" ID="Button1" CssClass="btn btn-primary " OnClick="Add_to_Favorites" runat="server" />
                <asp:Button Text="Statistics" class="btn btn-default unselectable" data-toggle="collapse" data-target="#StatsticsPanels" OnClientClick="return false;" runat="server" />
            </LoggedInTemplate>
        </asp:LoginView>
        <div id="AddToDropDown" class="navbar navbar-default panel-collapse collapse" style="z-index: 100">
            <div class="list-group">
                <asp:Repeater runat="server" ID="PlaylistRepeater" OnItemDataBound="PlaylistRepeater_ItemDataBound">
                    <ItemTemplate>
                        <asp:LinkButton ID="AddButton" CommandArgument='<%#Eval("Id") %>' CommandName="Add" OnCommand="AddToPlaylist_Command" CssClass="list-group-item" runat="server" Visible="false"><i class="glyphicon glyphicon-plus"></i><%#Eval("Name") %></asp:LinkButton>
                        <asp:LinkButton ID="RemoveButton" CommandArgument='<%#Eval("Id") %>' CommandName="Remove" OnCommand="AddToPlaylist_Command" CssClass="list-group-item active" runat="server" Visible="false"><i class="glyphicon glyphicon-ok"></i>Added to <%#Eval("Name") %></asp:LinkButton>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <%--<hr />--%>
            <asp:Button Text="Create a new Playlist" CssClass="btn btn-default" runat="server" data-toggle="collapse" data-target="#PLCreation" OnClientClick="return false;" /><br />
            <div id="PLCreation" class="panel-collapse collapse">
                <asp:Label ID="VidTitle_labe" runat="server" Text="Choose a name for your new playlist."></asp:Label><br />
                <asp:TextBox ID="PLTitle" runat="server" CssClass="textBoxes" Height="35px" Width="200px"></asp:TextBox><br />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="PLTitle" ValidationGroup="valGroup1" CssClass="text-danger" ErrorMessage="You must give a name to your playlist." />
                <asp:Button Text="Create" runat="server" class="btn btn-primary" ValidationGroup="valGroup1" OnClick="CreateNewPlaylist" />
            </div>
        </div>

        <%--        <input type="checkbox" id="like" />
        <label for="like">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                <path d="M12 21.35l-1.45-1.32c-5.15-4.67-8.55-7.75-8.55-11.53 0-3.08 2.42-5.5 5.5-5.5 1.74 0 3.41.81 4.5 2.09 1.09-1.28 2.76-2.09 4.5-2.09 3.08 0 5.5 2.42 5.5 5.5 0 3.78-3.4 6.86-8.55 11.54l-1.45 1.31z" />
            </svg>
        </label>--%>

        <div id="ProgressLikeSection" class="container" style="float: right" runat="server">
            <div class="row">
                <div class="col-md-1">
                    <asp:Label ID="upvote" runat="server" Text="0"></asp:Label>
                </div>
                <div class="col-md-8">
                    <button id="btn_like_ok" class='btn btn-sm btn-success' runat='server' onserverclick="Increment_like_video" visible="false"><span class='glyphicon glyphicon-thumbs-up'></span></button>
                    <button id="btn_like_neg" class='btn btn-sm btn-success closeBtn' runat='server' onserverclick="Increment_like_video" visible="false"><span class='glyphicon glyphicon-thumbs-up'></span></button>

                    <button id="btn_dislike_ok" class='btn btn-sm btn-danger' runat='server' onserverclick="Increment_dislike_video" visible="false"><span class='glyphicon glyphicon-thumbs-down'></span></button>
                    <button id="btn_dislike_neg" class='btn btn-sm btn-danger closeBtn' runat='server' onserverclick="Increment_dislike_video" visible="false"><span class='glyphicon glyphicon-thumbs-down'></span></button>
                </div>
                <div class="col-md-1">
                    <asp:Label ID="downvote" runat="server" Text="0"></asp:Label><br />
                </div>
            </div>
            <div class="progress" style="width: 150%; height: 5px; float: right">
                <asp:Panel ID="likes_progress" class="progress-bar" role="progressbar" aria-valuenow="20" aria-valuemin="0" aria-valuemax="100" runat="server">
                </asp:Panel>
                <asp:Panel ID="dislikes_progress" class="progress-bar progress-bar-danger" role="progressbar" aria-valuenow="20" aria-valuemin="0" aria-valuemax="100" runat="server">
                </asp:Panel>
            </div>
        </div>
        <br />
        <asp:Label ID="Date" runat="server" Text="Published on "></asp:Label><br />
        <div class="panel-collapse collapse in" id="Video_info_descr" style="min-height: 50px; padding-top: 20px">
            <asp:Label ID="Short_description" runat="server" Text="Description"></asp:Label>
            <asp:Label ID="vidTags" runat="server" Text="0/0"></asp:Label><br />
        </div>
        <button class="" data-toggle="collapse" data-target="#Video_info_descr" onclick="return false">
            <span class="fa-arrow-circle-down"></span>
        </button>

    </div>

    <div role="tabpanel" class="panel-collapse collapse" id="StatsticsPanels">

        <!-- Nav tabs -->
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">Views per day</a></li>
            <li role="presentation"><a href="#profile" aria-controls="profile" role="tab" data-toggle="tab">Likes per day</a></li>
            <li role="presentation"><a href="#messages" aria-controls="messages" role="tab" data-toggle="tab">Cumulative Views</a></li>
            <li role="presentation"><a href="#settings" aria-controls="settings" role="tab" data-toggle="tab">Settings</a></li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="home">
                <asp:Chart ID="Chart1" runat="server" DataSourceID="SqlDataSource1">
                    <Series>
                        <asp:Series Name="Series1" XValueMember="asd2" YValueMembers="asd1" YValuesPerPoint="3">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" SelectCommand="SELECT Count([User_id]) asd1, Convert(varchar(10),[Interaction_date],126) asd2 FROM [VideoStatistics] WHERE (([Video_id] = @Video_id) AND ([Interaction_type] = @Interaction_type)) GROUP BY Convert(varchar(10),[Interaction_date],126) ORDER BY Convert(varchar(10),[Interaction_date],126) DESC">
                    <SelectParameters>
                        <asp:QueryStringParameter DefaultValue="" Name="Video_id" QueryStringField="id" Type="Int32" />
                        <asp:Parameter DefaultValue="5" Name="Interaction_type" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
            <div role="tabpanel" class="tab-pane" id="profile">
                <asp:Chart ID="Chart2" runat="server" DataSourceID="SqlDataSource3">
                    <Series>
                        <asp:Series Name="Series1" XValueMember="asd2" YValueMembers="asd1" YValuesPerPoint="3">
                        </asp:Series>
                    </Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1">
                        </asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
                <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" SelectCommand="SELECT Convert(varchar(10),[Interaction_date],126) asd2, COUNT([Id]) asd1 FROM [VideoStatistics] WHERE (([Video_id] = @Video_id) AND ([Interaction_type] = @Interaction_type)) GROUP BY Convert(varchar(10),[Interaction_date],126) ORDER BY Convert(varchar(10),[Interaction_date],126) DESC">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="Video_id" QueryStringField="id" Type="Int32" />
                        <asp:Parameter DefaultValue="1" Name="Interaction_type" Type="Int32" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </div>
            <div role="tabpanel" class="tab-pane" id="messages">
                <asp:Chart ID="Chart3" runat="server" />
            </div>
            <div role="tabpanel" class="tab-pane" id="settings">...</div>
        </div>

    </div>










    <div class="row">
        <div class="col-md-8">
            <div id="comment_section">
                    <asp:ScriptManagerProxy runat="server"></asp:ScriptManagerProxy>
                    <asp:UpdatePanel runat="server" ID="UpdatePanelWatch" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="comment_input" class="form-inline" runat="server">
                            <div class="form-group">
                                <input id="Main_input" class="form-control" type="text" placeholder="Your comments" runat="server" style="height: 50px; width: 431px" />
                            </div>
                            <div class="form-group">
                                <asp:Button id="Refresh_Comments" ClientIDMode="Static" Text="Refresh" class="btn btn-default hide" runat="server" OnClick="Refresh_Comments_ServerClick" style="display:none" />
                                <asp:Button id="Add_Comment_Button" ClientIDMode="Static" Text="Add" class="btn btn-default" runat="server" OnClick="Post_comment"/>
                            </div>
                            </div>
                            <ul id="Comment_list" class="commentList">
                                <asp:Repeater ID="Wall_posts" runat="server" OnItemDataBound="R1_ItemDataBound">
                                    <ItemTemplate>
                                        <li>
                                            <div>
                                                <div class="commenterImage">
                                                    <a href='<%#Eval("User_posted","~/Account/Channel?id={0}") %>' runat="server">
                                                        <img src='<%#Eval("icon_path","{0}") %>' runat="server" />
                                                    </a>

                                                </div>

                                                <a href='<%#Eval("User_posted","~/Account/Channel?id={0}") %>' runat="server">
                                                    <p><%# Eval("UserName") %> </p>
                                                </a>
                                                <div id="<%#Eval("Id") %>">
                                                <asp:Button Text="&times;"  runat="server" ID="DeleteButton" CssClass='close' aria-hidden='true' CommandName="Delete" CommandArgument='<%#Eval("Id") %>' OnCommand="delete_post" Visible="false" />
                                                </div>
                                                <div class="commentText">
                                                    <p class="">
                                                        <%# Eval("Post_Text") %>
                                                    </p>
                                                    <span class="date sub-text">on <%# Eval("date_posted") %></span>
                                                    <div id="">
                                                        <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-sm btn-default initCmd" CommandArgument='<%#Eval("Id") %>' CommandName="Like" OnCommand="Increment_like_Post">
                                <i aria-hidden="true" class="glyphicon glyphicon-thumbs-up">
                                    <p style="color: green"><%#Eval("like") %> </p>
                                </i>
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="btnRandom" runat="server" CssClass="btn btn-sm btn-default initCmd" CommandArgument='<%#Eval("Id") %>' CommandName="Dislike" OnCommand="Increment_like_Post">
                                <i aria-hidden="true" class="glyphicon glyphicon-thumbs-down">
                                    <p style="color: red"><%#Eval("dislike") %> </p>
                                </i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ul>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>
            </div>
            <div class="col-md-4">
                <h4>Featured Videos</h4>
                <ul>
                    <asp:Repeater ID="Featured_Repeater" runat="server">
                        <ItemTemplate>
                            <li>
                                <div class="media">
                                    <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                        <div class="media-left">
                                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                                        </div>
                                        <div class="media-body">
                                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                                <h4 class="media-heading"><%#Eval("Title") %></h4>
                                            </a>
                                            <a href='<%#Eval("UserId","~/Account/Channel?id={0}") %>' runat="server"><span class="date sub-text">by <%# Eval("UserName") %></span> </a>
                                            <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                        </div>
                                    </a>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:LoginView runat="server">
                    <LoggedInTemplate>
                        <h4>Recomanded Videos</h4>
                    </LoggedInTemplate>
                </asp:LoginView>
                <ul>
                    <asp:Repeater ID="VIdPlayRep" runat="server">
                        <ItemTemplate>
                            <li>
                                <div class="media">
                                    <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                        <div class="media-left">
                                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                                        </div>
                                        <div class="media-body">
                                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                                <h4 class="media-heading"><%#Eval("Title") %></h4>
                                            </a>
                                            <a href='<%#Eval("UserId","~/Account/Channel?id={0}") %>' runat="server"><span class="date sub-text">by <%# Eval("UserName") %></span> </a>
                                            <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                        </div>
                                    </a>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
    </div>
        <br />
        <br />
        <br />
        <br />
        <br />

        <script>
            var links = document.getElementById("MainContent_Playlist_Contet").getElementsByTagName("a");
            var video = document.getElementById("MainContent_video_player_html5_api");
            video.addEventListener('ended', function () {
                var indexNr;
                var adress = location.search.substring(1);
                var params = adress.split("&");
                for (var i = 0; i < params.length; i++) {
                    var pair = params[i].split("=");
                    if (pair[0] == "index") {
                        indexNr = pair[1];
                    }
                }
                window.location.href = links[parseInt(indexNr)].href;
            })

        </script>
     
</asp:Content>
