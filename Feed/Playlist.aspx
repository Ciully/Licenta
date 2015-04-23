<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Playlist.aspx.cs" Inherits="Playlist" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="PlayListInfo" runat="server">
        <div class="media">
            <div class="media-left media-middle">
                <img class="media-object" runat="server" id="PlayThumb" />
            </div>
            <div class="media-body">
                <h3>
                    <asp:Label ID="PlaylistTitle" runat="server" Text="Label"></asp:Label></h3>
                <a id="UserLink" runat="server">
                    <asp:Label ID="UserName" runat="server" Text="Label"></asp:Label>
                </a>
                <asp:Label ID="NrVids" runat="server" Text="Label"></asp:Label>
                <asp:Label ID="DateCreated" runat="server" Text="Label"></asp:Label>
                <asp:Button ID="DeletePlaylist" runat="server" Text="Delete Playlist" OnClick="DeletePlaylist_Click" />
                <asp:Button ID="Button1" runat="server" Text="Play all" OnClick="Button1_Click" />
            </div>
        </div>
    </div>
    <hr />
    <div runat="server">
        <asp:Repeater ID="VIdPlayRep" runat="server">
            <ItemTemplate>
                <div class="media">
                    <div class="media-left">
                        <a href='<%#Eval("Id","~/watch?id={0}&index="+Eval("Index")+"&list="+Eval("PlayId"))%>' runat="server">
                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                        </a>
                    </div>
                    <div class="media-body">
                        <a href='<%#Eval("Id","~/watch?id={0}&index="+Eval("Index")+"&list="+Eval("PlayId"))  %>' runat="server">
                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                        </a>
                        <a href='<%#Eval("UserId","~/Account/Channel?id={0}") %>' runat="server">
                            <span class="date sub-text">by <%# Eval("UserName") %></span>
                        </a>
                        <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views  Added to Playlist on <%#Eval("Date_added_to_playlist") %></span>
                        <div style="height: 60px; overflow: hidden;"><%#Eval("Description") %></div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>


        <asp:Repeater ID="NonPlaylistRepeater" runat="server">
            <ItemTemplate>
                <div class="media">
                    <div class="media-left media-middle">
                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                            <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                        </a>
                    </div>
                    <div class="media-body">
                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                        </a>
                        <a href='<%#Eval("UserId","~/Account/Channel?id={0}") %>' runat="server">
                            <span class="date sub-text">by <%# Eval("UserName") %></span>
                        </a>
                        <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views  Added to Playlist on <%#Eval("Date_added_to_playlist") %></span>
                        <div style="height: 60px; overflow: hidden;"><%#Eval("Description") %></div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
