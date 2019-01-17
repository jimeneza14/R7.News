//
//  ViewAgent.ascx.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016-2017 Roman M. Yagodin
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using R7.Dnn.Extensions.ViewModels;
using R7.News.Agent.Components;
using R7.News.Agent.ViewModels;
using R7.News.Components;
using R7.News.Controls;
using R7.News.Data;
using R7.News.Models;
using R7.News.Modules;

namespace R7.News.Agent
{
    public partial class ViewAgent: NewsModuleBase<AgentSettings>, IActionable
    {
        #region Properties

        ViewModelContext<AgentSettings> viewModelContext;
        protected ViewModelContext<AgentSettings> ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext<AgentSettings> (this, Settings)); }
        }

        #endregion

        #region Handlers

        protected override void OnInit (EventArgs e)
        {
            AddActionHandler (OnAction);
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
                    Bind ();
                }
            }
            catch (Exception ex) {
                Exceptions.ProcessModuleLoadException (this, ex);
            }
        }

        #endregion

        protected void Bind ()
        {
            var items = NewsRepository.Instance.GetNewsEntriesByAgent (ModuleId, PortalId);

            // check if we have some content to display, 
            // otherwise display a message for module editors.
            if (items == null || !items.Any ()) {
                // show panel with add button
                if (IsEditable) {
                    panelAddDefaultEntry.Visible = true;
                }
                else {
                    // hide module from non-editors
                    ContainerControl.Visible = false;
                }
            }
            else {
                var now = HttpContext.Current.Timestamp;

                // create viewmodels
                var viewModels = items
                    .Where (ne => ne.IsPublished (now) || IsEditable)
                    .OrderByDescending (ne => ne.EntryId == Settings.GroupEntryId)
                    .ThenByDescending (ne => ne.PublishedOnDate ())
                    .GroupByAgentModule (Settings.EnableGrouping)
                    .Select (ne => new AgentNewsEntryViewModel (ne, ViewModelContext));
                
                // bind the data
                listAgent.DataSource = viewModels;
                listAgent.DataBind ();

                agplSignature.Visible = (Settings.EnableGrouping == false && listAgent.Items.Count > 1);
            }
        }

        #region IActionable implementation

        public override ModuleActionCollection ModuleActions
        {
            get {
                var actions = base.ModuleActions;

                actions.Add (
                    GetNextActionID (), 
                    LocalizeString ("AddFromTabData.Action"),
                    ModuleActionType.AddContent + "_AddFromTabData",
                    "",
                    IconController.IconURL ("Add"), 
                    "",
                    true,
                    SecurityAccessLevel.Edit,
                    true, 
                    false
                );

                // add edit action for each news entry
                var items = NewsRepository.Instance.GetNewsEntriesByAgent (ModuleId, PortalId);
                if (items != null && items.Any ()) {
                    foreach (var item in items) {
                        actions.Add (
                            GetNextActionID (), 
                            LocalizeString ("EditNewsEntry.Action") + HtmlUtils.Shorten (item.Title, 16,"&hellip;"),
                            ModuleActionType.EditContent,
                            "",
                            IconController.IconURL ("Edit", "16X16", "Gray"),
                            EditUrl ("entryid", item.EntryId.ToString (), "EditNewsEntry"),
                            false,
                            SecurityAccessLevel.Edit,
                            true, 
                            false
                        );
                    }
                }

                return actions;
            }
        }

        #endregion

        protected void OnAction (object sender, ActionEventArgs e)
        {
            if (e.Action.CommandName == ModuleActionType.AddContent + "_AddFromTabData") {
                AddFromTabData ();
            }
        }

        protected void buttonAddFromTabData_Click (object sender, EventArgs e)
        {
            AddFromTabData ();
        }

        protected void AddFromTabData ()
        {
            var newsEntry = AddFromTabData_Internal ();
            UpdateModuleTitle (newsEntry.Title);

            Response.Redirect (Globals.NavigateURL (), true);
        }

        private INewsEntry AddFromTabData_Internal ()
        {
            var tabController = new TabController ();
            var activeTab = tabController.GetTab (TabId, PortalId);

            // add default news entry based on tab data
            var newsEntry = new NewsEntryInfo {
                Title = activeTab.TabName,
                Description = HttpUtility.HtmlEncode ("<p>" + activeTab.Description + "</p>"),
                AgentModuleId = ModuleId,
                PortalId = PortalId,
                EndDate = DateTime.Today, // expired by default
                ThematicWeight = NewsConfig.Instance.NewsEntry.DefaultThematicWeight,
                StructuralWeight = NewsConfig.Instance.NewsEntry.DefaultStructuralWeight
            };

            // add news entry
            NewsRepository.Instance.AddNewsEntry (newsEntry, activeTab.Terms, new List<IFileInfo> (), ModuleId, TabId);

            return newsEntry;
        }

        /// <summary>
        /// Handles the items being bound to the listview control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listAgent_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (AgentNewsEntryViewModel) e.Item.DataItem;

            BindChildControls (item, e.Item);

            // show grouped news
            var listGroup = (ListView) e.Item.FindControl ("listGroup");
            var now = HttpContext.Current.Timestamp;

            if (item.Group != null && item.Group.Count > 0) {
                listGroup.DataSource = item.Group
                    .Where (ne => ne.IsPublished (now) || IsEditable)
                    .OrderByDescending (ne => ne.PublishedOnDate ())
                    .Select (ne => new AgentNewsEntryViewModel (ne, ViewModelContext));
                listGroup.DataBind ();
            }
        }

        /// <summary>
        /// Handles the items being bound to the listview control. In this method we merge the data with the
        /// template defined for this control to produce the result to display to the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void listGroup_ItemDataBound (object sender, ListViewItemEventArgs e)
        {
            var item = (AgentNewsEntryViewModel) e.Item.DataItem;

            // visibility badges
            var listBadges = (BadgeList) e.Item.FindControl ("listBadges");
            listBadges.DataSource = item.Badges;
            listBadges.DataBind ();
        }

        protected void UpdateModuleTitle (string title)
        {
            var moduleController = NewsDataProvider.Instance.ModuleController;
            var module = moduleController.GetModule (ModuleId, TabId, true);
            if (module.ModuleTitle != title) {
                module.ModuleTitle = title;
                moduleController.UpdateModule (module);
            }
        }
    }
}

