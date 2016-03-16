﻿//
//  ViewAgent.ascx.cs
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
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.R7;
using DotNetNuke.R7.Entities.Modules;
using R7.News.Models.Data;
using R7.News.Agent.Components;
using R7.News.Models;
using R7.News.Controls;
using R7.News.ViewModels;
using R7.News.Agent.ViewModels;

namespace R7.News.Agent
{
    public partial class ViewAgent : PortalModuleBase<AgentSettings>, IActionable
    {
        ViewModelContext<AgentSettings> viewModelContext;
        protected ViewModelContext<AgentSettings> ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<AgentSettings> (this, Settings)); }
        }

        #region Handlers

        /// <summary>
        /// Handles Load event for a control
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);
            
            try {
                if (!IsPostBack) {
                    
                    var items = NewsRepository.Instance.GetNewsEntriesByAgent (ModuleId);

                    // check if we have some content to display, 
                    // otherwise display a message for module editors.
                    if ((items == null || !items.Any ()) && IsEditable) {
                        this.Message ("NothingToDisplay.Text", MessageType.Info, true);
                    }
                    else {
                        // bind the data
                        listAgent.DataSource = items.Select (ne => new AgentModuleNewsEntryViewModel (ne, ViewModelContext));
                        listAgent.DataBind ();
                    }
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
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
                    Localization.GetString (ModuleActionType.AddContent, this.LocalResourceFile),
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
        protected void listAgent_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (AgentModuleNewsEntryViewModel) e.Item.DataItem;
            
            var linkEdit = (HyperLink) e.Item.FindControl ("linkEdit");
            var iconEdit = (Image) e.Item.FindControl ("imageEdit");
            
            // read module settings (may be useful in a future)
            // var settings = new R7.News.AgentSettings (this);            
            
            // edit link
            if (IsEditable) {
                linkEdit.NavigateUrl = EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry");
            }

            // make edit link visible in edit mode
            linkEdit.Visible = IsEditable;
            iconEdit.Visible = IsEditable;

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

