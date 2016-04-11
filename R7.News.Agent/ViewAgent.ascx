﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAgent.ascx.cs" Inherits="R7.News.Agent.ViewAgent" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Import Namespace="System.Web" %>

<asp:Panel id="panelAddDefaultEntry" runat="server" Visible="false" CssClass="dnnFormMessage dnnFormInfo">
    <asp:LinkButton id="buttonAddFromTabData" runat="server" resourcekey="buttonAddFromTabData.Text"
        CssClass="dnnSecondaryAction dnnRight" Style="position:relative;top:-.5em" OnClick="buttonAddFromTabData_Click" />
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
                <div class="col-sm-6">
                    <asp:Image id="imageImage" runat="server" ImageUrl='<%# Eval ("ImageUrl") %>'
                        CssClass="img img-rounded img-responsive" Style="margin-bottom:1em" />
                </div>
                <div class="col-sm-6">
                    <h3 style="margin-top:0">
                        <asp:HyperLink id="linkEdit" runat="server">
                            <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                        </asp:HyperLink>
                        <%# Eval ("Title") %>
                    </h3>
                    <news:BadgeList id="listBadges" runat="server" BadgeCssClass="badge" />
                    <p class="small" style="color:gray"><span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %>
                    <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Eval ("CreatedByUserName") %></p>
                    <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                    <p>
                        <news:TermLinks id="termLinks" runat="server" />
                    </p>
                    <asp:ListView id="listGroup" DataKeyNames="EntryId" runat="server" OnItemDataBound="listGroup_ItemDataBound">
                        <LayoutTemplate>
                            <div runat="server">
                                <div runat="server" id="itemPlaceholder"></div>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                                <h4>
                                    <asp:HyperLink id="linkEdit" runat="server">
                                        <asp:Image id="imageEdit" runat="server" IconKey="Edit" resourcekey="Edit" />
                                    </asp:HyperLink>
                                    <asp:LinkButton id="buttonTitle" runat="server" Text='<%# Eval ("Title") %>' 
                                        OnCommand="buttonTitle_Command" CommandArgument='<%# Eval ("EntryId") %>' />
                                </h4>
                                <news:BadgeList id="listBadges" runat="server" BadgeCssClass="badge" />
                                <p class="small" style="color:gray"><span class="glyphicon glyphicon-calendar"></span> <%# Eval ("PublishedOnDateString") %>
                                <span class="glyphicon glyphicon-user" style="margin-left:1em"></span> <%# Eval ("CreatedByUserName") %></p>
                            <div style="display:table-row">
                                <div style="display:table-cell">
                                    <asp:Image id="imageImage" runat="server" ImageUrl='<%# Eval ("GroupImageUrl") %>'
                                        CssClass="img" Style="margin-bottom:1em;margin-right:1em" />
                                </div>
                                <div style="display:table-cell;vertical-align:top;font-size:small">
                                    <%# HttpUtility.HtmlDecode ((string) Eval ("Description")) %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
            <div class="small">
                EntryId: <%# Eval ("EntryId") %><br />
                AgentModuleId: <%# Eval ("AgentModuleId") %><br />
                LastModifiedOnDate: <%# Eval ("ContentItem.LastModifiedOnDate") %><br />
                LastModifiedByUserID: <%# Eval ("ContentItem.LastModifiedByUserID") %>
            </div>
        </div>
    </ItemTemplate>
    <ItemSeparatorTemplate>
        <hr />
    </ItemSeparatorTemplate>
</asp:ListView>
