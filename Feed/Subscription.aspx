<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Subscription.aspx.cs" Inherits="Feed_Subscription" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div role="tabpanel" class="panel">

        <ul class="nav nav-tabs nav-pills" role="tablist">
            <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">All Activity</a></li>
            <li role="presentation"><a href="#profile" aria-controls="profile" role="tab" data-toggle="tab">Uploads</a></li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="home">
                <asp:Repeater ID="Activity_Repeater" runat="server">
                    <ItemTemplate>
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
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div role="tabpanel" class="tab-pane" id="profile">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <hr />
                        <div class="media">
                            <a href='<%#Eval("UserId","~/watch?id={0}") %>' runat="server">
                                <h4><%# Eval("UserName") %></h4>
                            </a>
                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                <div class="media-left">
                                    <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                                </div>
                                <div class="media-body">
                                    <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                        <h4 class="media-heading"><%#Eval("Title") %></h4>
                                    </a>
                                    <a href='<%#Eval("UserId","~/watch?id={0}") %>' runat="server">
                                        <span class="date sub-text">by <%# Eval("UserName") %></span>
                                    </a>
                                    <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                    <%#Eval("Description") %>
                                </div>
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </div>





</asp:Content>
