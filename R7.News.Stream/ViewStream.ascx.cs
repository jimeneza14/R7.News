﻿//
//  ViewStream.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.R7;
using DotNetNuke.R7.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using R7.News.Controls;
using R7.News.Models;
using R7.News.Data;
using R7.News.Stream.Components;
using R7.News.Stream.ViewModels;
using R7.News.ViewModels;
using PagingControlMode = DotNetNuke.R7.PagingControlMode;
using System.Runtime.InteropServices;

namespace R7.News.Stream
{
    public partial class ViewStream : PortalModuleBase<StreamSettings>, IActionable
    {
        ViewModelContext<StreamSettings> viewModelContext;
        protected ViewModelContext<StreamSettings> ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<StreamSettings> (this, Settings)); }
        }

        protected int PageSize = 2;

        protected int PageNumber = 1;

        #region Handlers

        protected override void OnInit (EventArgs e)
        {
            base.OnInit (e);

            // setup paging control
            pagingControl.CurrentPage = 1;
            pagingControl.TabID = TabId;
            pagingControl.PageSize = PageSize;
            pagingControl.Mode = PagingControlMode.PostBack;
            pagingControl.QuerystringParams = "pagingModuleId=" + ModuleId;
        }

        /// <summary>
        /// Handles Load event for a control
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            try {
                if (!IsPostBack) {
                    int itemsCount;
                    var pagedItems = GetPagedItems (pagingControl.CurrentPage - 1, out itemsCount);

                    // setup paging control
                    pagingControl.TotalRecords = itemsCount;

                    if (itemsCount > 0) {
                        // bind the data
                        listStream.DataSource = pagedItems;
                        listStream.DataBind ();
                    }
                    else if (IsEditable) {
                        this.Message ("NothingToDisplay.Text", MessageType.Info, true);
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        protected void pagingControl_PageChanged (object sender, EventArgs e)
        {
            int itemsCount;
            var pagedItems = GetPagedItems (pagingControl.CurrentPage - 1, out itemsCount);

            // setup paging control
            pagingControl.TotalRecords = itemsCount;
        
            if (itemsCount > 0) {
                // bind the data
                listStream.DataSource = pagedItems;
                listStream.DataBind ();
            }
        }

        protected IList<StreamModuleNewsEntryViewModel> GetPagedItems (int pageIndex, out int itemsCount)
        {
            IEnumerable<ModuleNewsEntryInfo> items;

            if (Settings.ShowAllNews) {
                items = NewsRepository.Instance.GetModuleNewsEntries (ModuleId, PortalId);
            }
            else {
                items = NewsRepository.Instance.GetModuleNewsEntriesByTerms (ModuleId, PortalId, Settings.IncludeTerms);
            }

            // TODO: Implement check for pageIndex > totalPages

            if (pageIndex < 0 || items == null || !items.Any ()) {

                    itemsCount = 0;
                    return null;
            }
     
            itemsCount =  items.Count ();

            return items.OrderByDescending (ne => ne.ContentItem.CreatedOnDate)
                .Skip (pageIndex * PageSize)
                .Take (PageSize)
                .Select (ne => new StreamModuleNewsEntryViewModel (ne, ViewModelContext))
                .ToList ();
            
        }

        #endregion

        #region IActionable implementation

        public ModuleActionCollection ModuleActions
        {
            get {
                // create a new action to add an item, this will be added 
                // to the controls dropdown menu
                var actions = new ModuleActionCollection ();
                actions.Add (
                    GetNextActionID (), 
                    LocalizeString (ModuleActionType.AddContent),
                    ModuleActionType.AddContent,
                    "", 
                    "", 
                    EditUrl ("EditNewsEntry"),
                    false, 
                    DotNetNuke.Security.SecurityAccessLevel.Edit,
                    true, 
                    false
                );

                return actions;
            }
        }

        #endregion

        /// <summary>
        /// Handles the items being bound to the datalist control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listStream_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (StreamModuleNewsEntryViewModel) e.Item.DataItem;

            var linkEdit = (HyperLink) e.Item.FindControl ("linkEdit");
            var iconEdit = (Image) e.Item.FindControl ("imageEdit");

            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

            // visibility badges
            var listBadges = (BadgeList) e.Item.FindControl ("listBadges");
            List<Badge> badges = null;

            if (IsEditable) {

                badges = new List<Badge> ();
            
                if (!item.IsPublished ()) {
                    if (item.HasBeenExpired ()) {
                        badges.Add (new Badge {
                            CssClass = "expired",
                            Text = string.Format (LocalizeString ("Visibility_Expired.Format"), item.EndDate)
                        });
                    }
                    else {
                        badges.Add (new Badge {
                            CssClass = "not-published",
                            Text = string.Format (LocalizeString ("Visibility_NotPublished.Format"), item.StartDate)
                        });
                    }
                }

                if (item.GetNewsEntryVisibility () == NewsEntryVisibility.Hidden) {
                    badges.Add (new Badge {
                        CssClass = "is-hidden",
                        Text = LocalizeString ("Visibility_Hidden.Text")
                    });
                }
                else if (item.GetNewsEntryVisibility () == NewsEntryVisibility.DefaultHidden) {
                    badges.Add (new Badge {
                        CssClass = "default-hidden",
                        Text = LocalizeString ("Visibility_DefaultHidden.Text")
                    });
                }
            }

            listBadges.DataSource = badges;
            listBadges.DataBind ();

            // show image
            var imageImage = (Image) e.Item.FindControl ("imageImage");
            imageImage.Visible = item.GetImage () != null;

            // show term links
            var termLinks = (TermLinks) e.Item.FindControl ("termLinks");
            termLinks.Module = this;
            termLinks.DataSource = item.ContentItem.Terms;
            termLinks.DataBind ();
        }
    }
}

