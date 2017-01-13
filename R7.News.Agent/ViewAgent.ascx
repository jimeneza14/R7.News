﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAgent.ascx.cs" Inherits="R7.News.Agent.ViewAgent" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Import Namespace="System.Web" %>

<asp:Panel id="panelAddDefaultEntry" runat="server" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:LinkButton id="buttonAddFromTabData" runat="server" resourcekey="buttonAddFromTabData.Text"
        CssClass="dnnSecondaryAction dnnRight button-add-from-tab-data" OnClick="buttonAddFromTabData_Click" />
    <%: LocalizeString ("NothingToDisplay.Text") %>
</asp:Panel>
<asp:ListView id="listAgent" DataKeyNames="EntryId" runat="server" OnItemDataBound="listAgent_ItemDataBound">
    <LayoutTemplate>
        <div runat="server" class="news-agent">
            <div runat="server" id="itemPlaceholder"></div>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <div>
            <div class="row">
                <div class="<%# Eval ("FirstColumnContainerCssClass") %>">
                    <asp:HyperLink id="linkImage" runat="server" NavigateUrl='<%# Eval ("Link") %>' Visible='<%# Eval ("HasImage") %>'>
                        <asp:Image id="imageImage" runat="server" 
                            ImageUrl='<%# Eval ("ImageUrl") %>' AlternateText='<%# Eval ("Title") %>'
                            CssClass="img img-rounded img-responsive" />
                    </asp:HyperLink>
                </div>
                <div class="<%# Eval ("SecondColumnContainerCssClass") %>">
                    <h3 style="margin-top:0">
                        <asp:HyperLink id="linkEdit" runat="server">
                            <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                        </asp:HyperLink>
                        <%# HttpUtility.HtmlDecode ((string) Eval ("TitleLink")) %>
                    </h3>
                    <news:BadgeList id="listBadges" runat="server" CssClass="visibility-badges" BadgeCssClass="badge" />
                    <p class="news-entry-info">
                        <span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %>
                        <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Eval ("CreatedByUserName") %></p>
                    <p>
                    <div class="news-entry-description">
                        <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                    </div>
                    <news:TermLinks id="termLinks" runat="server" CssClass="term-links" />
                    <asp:ListView id="listGroup" DataKeyNames="EntryId" runat="server" OnItemDataBound="listGroup_ItemDataBound">
                        <LayoutTemplate>
                            <div runat="server" class="list-group">
                                <div runat="server" id="itemPlaceholder"></div>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <h4>
                                <asp:HyperLink id="linkEdit" runat="server">
                                    <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                                </asp:HyperLink>
								<%# HttpUtility.HtmlDecode ((string) Eval ("TitleLink")) %>
                            </h4>
                            <news:BadgeList id="listBadges" runat="server" CssClass="visibility-badges" BadgeCssClass="badge" />
                            <p class="news-entry-info">
                                <span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %>
                                <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Eval ("CreatedByUserName") %>
                            </p>
                            <div class="news-entry-main-row">
                                <div>
                                    <asp:HyperLink id="linkImage" runat="server" NavigateUrl='<%# Eval ("Link") %>' Visible='<%# Eval ("HasImage") %>'>
                                        <asp:Image id="imageImage" runat="server" 
                                            ImageUrl='<%# Eval ("GroupImageUrl") %>' AlternateText='<%# Eval ("Title") %>'
                                            CssClass="img news-entry-image" />
                                    </asp:HyperLink>
                                </div>
                                <div class="news-entry-description">
                                    <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
