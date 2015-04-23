<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AdminControlPanel.aspx.cs" Inherits="Account_AdminControlPanel" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div role="tabpanel">

        <!-- Nav tabs -->
        <ul class="nav nav-tabs" id="myTab" role="tablist">
            <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">Home</a></li>
            <li role="presentation"><a href="#profile" aria-controls="profile" role="tab" data-toggle="tab">Manage Users</a></li>
            <li role="presentation"><a href="#messages" aria-controls="messages" role="tab" data-toggle="tab">Manage Videos</a></li>
            <li role="presentation"><a href="#settings" aria-controls="settings" role="tab" data-toggle="tab">Delete/Add Categories</a></li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="home">
                <asp:Panel runat="server">
                    <asp:Chart ID="Chart2" runat="server" DataSourceID="SqlDataSource2">
                        <Series>
                            <asp:Series Name="Series1" XValueMember="Title" YValueMembers="Views" YValuesPerPoint="3">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea2">
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" SelectCommand="SELECT [Title], [Views] FROM [Videos] WHERE ([Views] &gt; @Views) ORDER BY [Views] DESC">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="0" Name="Views" Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>


                    <asp:Chart ID="Chart3" runat="server" DataSourceID="SqlDataSource1">
                        <Series>
                            <asp:Series Name="Series2" ChartType="Pie" XValueMember="Countries" YValueMembers="nrUsr">
                            </asp:Series>
                        </Series>
                        <ChartAreas>
                            <asp:ChartArea Name="ChartArea1">
                            </asp:ChartArea>
                        </ChartAreas>
                    </asp:Chart>
                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" SelectCommand="SELECT COUNT([UserName]) nrUsr, [Country] Countries FROM [AspNetUsers] GROUP BY [Country]"></asp:SqlDataSource>


                </asp:Panel>
            </div>
            <div role="tabpanel" class="tab-pane" id="profile">
                <asp:UpdatePanel runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="input-group">
                            <asp:TextBox runat="server" class="form-control" placeholder="Search for an user" aria-describedby="basic-addon2" ID="UserSrc" />
                            <button class="input-group-addon" runat="server" onserverclick="UserSearch_ServerClick" onclientclick="return false;" id="basic_addon2">Search</button>
                        </div>
                        <ul>
                            <asp:Repeater ID="UsersRepeater" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <div>
                                            <div class="commenterImage">
                                                <a href='<%#Eval("Id","~/Account/Channel?id={0}") %>' runat="server">
                                                    <img src='<%#Eval("icon_path","{0}") %>' runat="server" />
                                                </a>
                                            </div>
                                            <a href='<%#Eval("Id","~/Account/Channel?id={0}") %>' runat="server">
                                                <p><%# Eval("UserName") %> </p>
                                            </a>
                                            <span class="date sub-text">Member since <%# Eval("Date_created") %></span>
                                            <asp:Button CssClass="close" Text="Delete User" OnCommand="DeleteUser_Command" CommandArgument='<%# Eval("Id") %>' CommandName="Delete" runat="server" />
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div role="tabpanel" class="tab-pane" id="messages">
                <asp:UpdatePanel runat="server" UpdateMode="Always">
                    <ContentTemplate>
                        <div class="modal fade" id="Save_modal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                            <div class="modal-dialog"></div></div>
                        <div class="modal fade" id="aditional_info" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <h4 class="modal-title">
                                            <asp:Label runat="server" ID="VidTitleLabel"></asp:Label></h4>
                                    </div>
                                    <div class="modal-body">
                                        <asp:Label ID="VidTitle_labe" runat="server" Text="Give your video a title."></asp:Label><br />
                                        <asp:TextBox ID="VidTitle" runat="server" CssClass="textBoxes" Height="35px" Width="634px"></asp:TextBox><br />

                                        <asp:Label ID="VidDescr_label" runat="server" Text="Add a description."></asp:Label><br />
                                        <asp:TextBox ID="VideoDescription" runat="server" CssClass="textBoxes" Height="128px" Width="631px"></asp:TextBox><br />

                                        <asp:Label ID="VidTags" runat="server" Text="Add Tags."></asp:Label><br />
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
                        <asp:Repeater ID="video_list" runat="server">
                            <ItemTemplate>
                                <div class="media">
                                    <div class="media-left">
                                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                                        </a>
                                        &nbsp;&nbsp;&nbsp;
                                    </div>
                                    <div class="media-body">
                                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                                        </a>
                                        <span class="date sub-text">on <%# Eval("date_posted") %></span>
                                        <div id='btn_update<%# Eval("Id") %>'>
                                            <asp:Button ID="Button1" Text="Update" runat="server" CssClass="btn btn-default Updatas" CommandName="Update" OnCommand="CommandBtn_Click" CommandArgument='<%#Eval("Id") + "#" + Eval("Title") + "#" + Eval("Description") + "#" + Eval("Tags")%>' />
                                            <asp:Button ID="Button2" Text="Delete" runat="server" CssClass="btn btn-danger close" CommandName="Delete" OnCommand="CommandBtn_Click" CommandArgument='<%#Eval("Id")%>' />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div role="tabpanel" class="tab-pane" id="settings">
                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="input-group">
                            <%--<input type="text" class="form-control" placeholder="Search for an user" aria-describedby="basic-addon2" runat="server" id="CategoryInput">--%>
                            <asp:TextBox class="form-control" placeholder="New Category Name" aria-describedby="basic-addon2" runat="server" ID="CategoryInput" />
                            <button class="input-group-addon" runat="server" onserverclick="CategoryAdd_ServerClick" id="Button4">Add a new Category</button>
                        </div>
                        <ul>
                            <asp:Repeater ID="Categ_Repeater" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <%#Eval("categ_name") %>
                                        <asp:Button Text="Delete Category" runat="server" CommandName="Category_Delete" OnCommand="Category_Command" CommandArgument='<%#Eval("Id")%>' />
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <asp:HiddenField ID="hidTAB" runat="server" Value="home" />
</asp:Content>

