﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewAgent.ascx.cs" Inherits="R7.News.Agent.ViewAgent" %>
<%@ Register TagPrefix="news" TagName="TermLinks" Src="~/DesktopModules/R7.News/R7.News/Controls/TermLinks.ascx" %>
<%@ Register TagPrefix="news" TagName="BadgeList" Src="~/DesktopModules/R7.News/R7.News/Controls/BadgeList.ascx" %>
<%@ Register TagPrefix="news" TagName="ActionButtons" Src="~/DesktopModules/R7.News/R7.News/Controls/ActionButtons.ascx" %>
<%@ Register TagPrefix="news" TagName="AgplSignature" Src="~/DesktopModules/R7.News/R7.News/Controls/AgplSignature.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>
<%@ Import Namespace="System.Web" %>
<dnn:DnnCssInclude runat="server" FilePath="~/DesktopModules/R7.News/R7.News/assets/css/module.css" />
<asp:Panel id="panelAddDefaultEntry" runat="server" Visible="false" CssClass="alert alert-warning">
	<div class="d-flex">
	    <div class="flex-grow-1 align-self-center">
			<%: LocalizeString ("NothingToDisplay.Text") %>
		</div>
		<asp:LinkButton id="buttonCreateFromPageData" runat="server" resourcekey="buttonCreateFromPageData.Text"
	        CssClass="btn btn-warning" OnClick="buttonCreateFromPageData_Click" />
	</div>
</asp:Panel>
<asp:ListView id="listAgent" ItemType="R7.News.Agent.ViewModels.AgentNewsEntryViewModel" runat="server" OnItemDataBound="listAgent_ItemDataBound">
    <LayoutTemplate>
        <div runat="server" class="news-module news-agent">
            <div runat="server" id="itemPlaceholder"></div>
        </div>
    </LayoutTemplate>
    <ItemTemplate>
        <div>
            <div class="row">
                <div class="<%# Item.ImageColumnCssClass %>">
                    <asp:HyperLink id="linkImage" runat="server" NavigateUrl="<%# Item.Link %>" Visible="<%# Item.HasImage %>">
                        <asp:Image id="imageImage" runat="server"
                            ImageUrl="<%# Item.ImageUrl %>" AlternateText="<%# Item.Title %>"
                            CssClass='<%# Item.ImageCssClass + " news-entry-image" %>' />
                    </asp:HyperLink>
				</div>
                <div class='<%# Item.TextColumnCssClass + " news-entry-text-column" %>'>
                    <h3 class="mt-0">
                        <%# HttpUtility.HtmlDecode (Item.TitleLink) %>
                    </h3>
					<p>
						<news:TermLinks id="termLinks" runat="server" CssClass="list-inline term-links" />
					</p>
                    <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="list-inline-item badge" />
                    <ul class="list-inline news-entry-info">
                        <li class="list-inline-item"><i class="fas fa-calendar-alt"></i> <%# Item.PublishedOnDateString %></li>
                        <li class="list-inline-item"><i class="fas fa-user"></i> <%# Item.CreatedByUserName %></li>
                    </ul>
                    <div class="<%# Item.TextCssClass %>">
                        <%# HttpUtility.HtmlDecode (Item.Description) %>
					</div>
			        <ul class="list-inline news-action-btns">
						<li runat="server" class="list-inline-item dropdown" Visible="<%# IsEditable %>">
                            <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-toggle="dropdown"><i class="fas fa-cog"></i></button>
                            <div class="dropdown-menu">
						    	<a class="dropdown-item" href='<%# HttpUtility.HtmlAttributeEncode (EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry")) %>'>
									<i class="fas fa-pencil-alt"></i>
									<%# LocalizeString ("EditNewsEntry.Text") %>
								</a>
								<div class="dropdown-divider"></div>
								<asp:LinkButton runat="server" CssClass="dropdown-item" OnCommand="btnDuplicate_Command"
										CommandName="Duplicate" CommandArgument="<%# Item.EntryId.ToString () %>">
									<i class="far fa-clone"></i>
									<%# LocalizeString ("Duplicate.Text") %>
                                </asp:LinkButton>
								<div class="dropdown-divider"></div>
								<asp:LinkButton runat="server" CssClass="dropdown-item" OnCommand="btnSyncTab_Command"
										CommandName="SyncTab" CommandArgument="<%# Item.EntryId.ToString () %>">
									<i class="fas fa-sync"></i>
									<%# LocalizeString ("SyncTab.Text") %>
                                </asp:LinkButton>
							</div>
						</li>
						<news:ActionButtons id="actionButtons" runat="server" />
					</ul>
                    <asp:ListView id="listGroup" ItemType="R7.News.Agent.ViewModels.AgentNewsEntryViewModel" runat="server" OnItemDataBound="listGroup_ItemDataBound">
                        <LayoutTemplate>
                            <div runat="server" class="list-group">
                                <div runat="server" id="itemPlaceholder"></div>
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <h4><%# HttpUtility.HtmlDecode (Item.TitleLink) %></h4>
                            <news:BadgeList id="listBadges" runat="server" CssClass="list-inline visibility-badges" BadgeCssClass="list-inline-item badge" />
                            <ul class="list-inline news-entry-info">
                                <li class="list-inline-item"><i class="fas fa-calendar-alt"></i> <%# Item.PublishedOnDateString %></li>
                                <li class="list-inline-item"><i class="fas fa-user"></i> <%# Item.CreatedByUserName %></li>
                            </ul>
                            <div class="news-entry-main-row">
                                <div>
                                    <asp:HyperLink id="linkImage" runat="server" NavigateUrl="<%# Item.Link %>" Visible="<%# Item.HasImage %>">
                                        <asp:Image id="imageImage" runat="server"
                                            ImageUrl="<%# Item.GroupImageUrl %>" AlternateText="<%# Item.Title %>"
                                            CssClass='<%# Item.ImageCssClass + " news-entry-image" %>' />
                                    </asp:HyperLink>
                                </div>
                                <div class="news-entry-text-column">
                                    <div class="<%# Item.TextCssClass %>">
                                        <%# HttpUtility.HtmlDecode (Item.Description) %>
									</div>
									<ul class="list-inline news-action-btns">
										<li runat="server" class="list-inline-item dropdown" Visible="<%# IsEditable %>">
				                            <button class="btn btn-outline-secondary btn-sm dropdown-toggle" type="button" data-toggle="dropdown"><i class="fas fa-cog"></i></button>
				                            <div class="dropdown-menu">
												<a class="dropdown-item" href='<%# HttpUtility.HtmlAttributeEncode (EditUrl ("entryid", Item.EntryId.ToString (), "EditNewsEntry")) %>'>
													<i class="fas fa-pencil-alt"></i>
													<%# LocalizeString ("EditNewsEntry.Text") %>
												</a>
											</div>
										</li>
									</ul>
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
<news:AgplSignature id="agplSignature" runat="server" />
