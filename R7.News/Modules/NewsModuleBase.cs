﻿//
//  NewsModuleBase.cs
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

using System.Web.UI.WebControls;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Log.EventLog;
using R7.DotNetNuke.Extensions.Modules;
using R7.News.Components;
using R7.News.Data;
using R7.News.Providers.DiscussProviders;
using System.Linq;

namespace R7.News.Modules
{
    public abstract class NewsModuleBase<TSettings>: PortalModuleBase<TSettings>, IActionable
        where TSettings: class, new ()
    {
        #region IActionable implementation

        public virtual ModuleActionCollection ModuleActions {
            get {
                var actions = new ModuleActionCollection ();
                actions.Add (
                    GetNextActionID (),
                    LocalizeString ("AddNewsEntry.Action"),
                    ModuleActionType.AddContent,
                    "",
                    IconController.IconURL ("Add"),
                    EditUrl ("EditNewsEntry"),
                    false,
                    SecurityAccessLevel.Edit,
                    true,
                    false
                );

                return actions;
            }
        }

        #endregion

        protected void linkDiscuss_Command (object sender, CommandEventArgs e)
        {
            var entryId = int.Parse ((string)e.CommandArgument);
            var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, PortalId);
            var discussProvider = NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault ();
            if (newsEntry != null && discussProvider != null) {
                var discussId = discussProvider.Discuss (newsEntry, PortalId, UserId);

                if (discussId > 0) {
                    Response.Redirect (discussProvider.GetDiscussUrl (discussId));
                }
                else {
                    var log = new LogInfo ();
                    log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
                    log.LogPortalID = PortalId;
                    log.AddProperty ("Message", "Cannot create discussion for a news entry");
                    EventLogController.Instance.AddLog (log);
                }
            }
        }
    }
}
