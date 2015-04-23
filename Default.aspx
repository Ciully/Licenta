<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <div class="panel container">
        <div class="row container">
            <h2>Spotlight</h2>
            <div class="col-md-6">
                <a id="Main_spot_link" href="#" runat="server">
                    <img id="Main_spot_thumb" src="..." alt="..." runat="server" />
                    <h4 class="media-heading" id="Spot_title" runat="server"></h4>
                </a>
                <div class="media-body">
                    <span class="date sub-text">9
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
            <div class="col-md-6">
                <ul class="list-group nav" style="list-style: none">

                    <asp:Repeater runat="server" ID="SpotLight_Repeater">
                        <ItemTemplate>
                            <li>
                                <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                    <div class="media">
                                        <div class="media-left media-middle">
                                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                                        </div>
                                        <div class="media-body">
                                                <h4 class="media-heading"><%#Eval("Title") %></h4>
                                                <span class="date sub-text">by <%# Eval("UserName") %></span>
                                            <br />
                                            <span class="date sub-text">on <%# Eval("date_posted") %> <b><%#Eval("Views") %> Views</b> </span>
                                        </div>
                                    </div>
                                </a>

                                <hr />
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-8 panel">
            <ul class="nav">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <li>
                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                <div class="media">
                                    <div class="media-left media-middle">
                                        <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                                    </div>
                                    <div class="media-body">
                                        <h4 class="media-heading"><%#Eval("Title") %></h4>
                                        <span class="date sub-text">by <%# Eval("UserName") %></span>
                                        <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                        <div style="height: 60px; overflow: hidden;"><%#Eval("Description") %></div>
                                    </div>
                                </div>
                            </a>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div class="col-md-4 panel">
            <h4>Recommended Videos</h4>
            <ul class="list-group nav">
                <asp:Repeater ID="VIdPlayRep" runat="server">
                    <ItemTemplate>
                        <li class="list-group-item">
                            <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                                <div class="media">
                                    <div class="media-left media-middle">
                                        <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image" />
                                    </div>
                                    <div>
                                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                                            <span class="date sub-text">by <%# Eval("UserName") %></span> 
                                        <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                                    </div>
                                </div>
                            </a>

                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
    </div>
</asp:Content>
