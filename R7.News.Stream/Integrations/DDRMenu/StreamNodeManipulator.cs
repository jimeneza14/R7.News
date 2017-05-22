﻿//
//  StreamNodeManipulator.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2017 Roman M. Yagodin
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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Web.DDRMenu;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;

namespace R7.News.Stream.Integrations.DDRMenu
{
    public class StreamNodeManipulator: INodeManipulator
    {
        #region INodeManipulator implementation

        // TODO: Pass parameters via specific node, remove it after processing?
        public List<MenuNode> ManipulateNodes (List<MenuNode> nodes, PortalSettings portalSettings)
        {
            try {
                var config = NewsConfig.GetInstance (portalSettings.PortalId).NodeManipulator;
                var parentNode = nodes.FirstOrDefault (n => n.TabId == config.ParentNodeTabId);
                if (parentNode != null) {
                    var streamModule = ModuleController.Instance.GetModule (config.StreamModuleId, config.StreamModuleTabId, false);
                    if (streamModule != null) {
                        var settingsRepository = new StreamSettingsRepository ();
                        var settings = settingsRepository.GetSettings (streamModule);
                        var newsEntries = GetNewsEntries (settings, settings.PageSize, portalSettings.PortalId);
                        foreach (var newsEntry in newsEntries) {
                            parentNode.Children.Add (CreateMenuNode (newsEntry, parentNode, streamModule));
                        }
                    } else {
                        LogAdminAlert ($"Could not find Stream module with ModuleID={config.StreamModuleId} on page with TabID={config.StreamModuleTabId}.", portalSettings.PortalId);
                    }
                } else {
                    LogAdminAlert ($"Could not find parent node with TabID={config.ParentNodeTabId}.", portalSettings.PortalId);
                }
            } catch (Exception ex) {
                Exceptions.LogException (ex);
            }

            return nodes;
        }

        #endregion

        protected IEnumerable<INewsEntry> GetNewsEntries (StreamSettings settings, int newsCount, int portalId)
        {
            // TODO: Cache the result?
            if (settings.ShowAllNews) {
                return NewsRepository.Instance.GetNewsEntries_FirstPage (
                    portalId, newsCount, DateTime.Now,
                    new WeightRange (settings.MinThematicWeight, settings.MaxThematicWeight),
                    new WeightRange (settings.MinStructuralWeight, settings.MaxStructuralWeight)
                );
            }

            return NewsRepository.Instance.GetNewsEntriesByTerms_FirstPage (
                portalId, newsCount, DateTime.Now,
                new WeightRange (settings.MinThematicWeight, settings.MaxThematicWeight),
                new WeightRange (settings.MinStructuralWeight, settings.MaxStructuralWeight),
                settings.IncludeTerms
            );
        }

        protected MenuNode CreateMenuNode (INewsEntry newsEntry, MenuNode parentNode, ModuleInfo streamModule)
        {
            var node = new MenuNode ();
            node.Enabled = true;
            node.Parent = parentNode;
            node.Text = newsEntry.Title;
            node.Title = newsEntry.Title;
            node.Description = HtmlUtils.StripTags (HttpUtility.HtmlDecode (newsEntry.Description), false);
            node.Url = newsEntry.GetUrl (streamModule.TabID, streamModule.ModuleID);

            if (newsEntry.AgentModule != null) {
                node.TabId = newsEntry.AgentModule.TabID;
            }

            return node;
        }

        void LogAdminAlert (string message, int portalId)
        {
            var log = new LogInfo ();
            log.LogPortalID = portalId;
            log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
            log.AddProperty (GetType ().ToString (), message);
            EventLogController.Instance.AddLog (log);
        }
    }
}
