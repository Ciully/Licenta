<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="ManageSubscriptions.aspx.cs" Inherits="ManageSubscriptions" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div role="tabpanel">

        <!-- Nav tabs -->
        <ul class="nav nav-tabs nav-pills" role="tablist">
            <li role="presentation" class="active"><a href="#home" aria-controls="home" role="tab" data-toggle="tab">Subscriptions</a></li>
            <li role="presentation"><a href="#profile" aria-controls="profile" role="tab" data-toggle="tab">Subscribers</a></li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="home">
                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ul class="commentList">
                            <asp:Repeater ID="Subs" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <div class="commenterImage">
                                            <a href='<%#Eval("UID","~/Account/Channel?id={0}")%>'>
                                                <img src='<%# Eval("Icon")%>' runat="server" />
                                            </a>
                                        </div>
                                        <a href='<%#Eval("UID","~/Account/Channel?id={0}")%>'>
                                            <p><%#Eval("Name")%></p>
                                        </a>
                                        <asp:Button ID="Unsub" Text="Unsubscribe" CssClass="close" OnCommand="Unsub_Command" CommandArgument='<%#Eval("Id")%>' CommandName="Unsub" runat="server" />
                                        <hr />                                    
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>


            <div role="tabpanel" class="tab-pane" id="profile">
                <asp:UpdatePanel runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <ul class="commentList">
                            <asp:Repeater ID="Subers" runat="server">
                                <ItemTemplate>
                                    <li>
                                        <div class="commenterImage">
                                            <a href='<%#Eval("UID","~/Account/Channel?id={0}")%>'>
                                                <img src='<%# Eval("Icon")%>' runat="server" />
                                            </a>
                                        </div>
                                        <a href='<%#Eval("UID","~/Account/Channel?id={0}")%>'>
                                            <p><%#Eval("Name")%></p>
                                        </a>
                                        <asp:Button ID="RemoveSub" CssClass="close" Text="Remove" OnCommand="RemoveSub_Command" CommandArgument='<%#Eval("Id")%>' CommandName="Remove_suber" runat="server" />
                                        <hr />
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

        </div>

    </div>

</asp:Content>

