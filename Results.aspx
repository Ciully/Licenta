<%@ Page Language="C#" MasterPageFile="~/Site.Master"  AutoEventWireup="true" CodeFile="Results.aspx.cs" Inherits="Results" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
       <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <div class="media">
                <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                    <div class="media-left">
                        <img class="media-object" src='<%#Eval("Thumbnail") %>' runat="server" alt="No Image">
                    </div>
                    <div class="media-body">
                        <a href='<%#Eval("Id","~/watch?id={0}") %>' runat="server">
                            <h4 class="media-heading"><%#Eval("Title") %></h4>
                        </a>
                        <a href='<%#Eval("UserId","~/watch?id={0}") %>' runat="server">
                            <span class="date sub-text">by <%# Eval("UserName") %></span>
                        </a>
                        <%#Eval("Description") %>
                        <span class="date sub-text">on <%# Eval("date_posted") %>  <%#Eval("Views") %> Views</span>
                    </div>
                </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>

</asp:Content>
