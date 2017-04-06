﻿//
//  ActionHandler.cs
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
using System.Linq;
using System.Web;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using R7.News.Data;
using R7.News.Models;
using R7.News.Providers.DiscussProviders;

namespace R7.News.Components
{
    public class ActionHandler
    {
        static readonly object discussLock = new object ();

        public void ExecuteAction (string actionKey, int entryId, int portalId, int userId)
        {
            try {
                if (actionKey.StartsWith ("Discuss", StringComparison.InvariantCulture)) {
                    ExecuteDiscussAction (actionKey, entryId, portalId, userId);
                }
                else if (actionKey.StartsWith ("JoinDiscussion", StringComparison.InvariantCulture)) {
                    ExecuteJoinDiscussionAction (entryId, portalId);
                }
            }
            catch (Exception ex) {
                var log = new LogInfo ();
                log.Exception = new ExceptionInfo (ex);
                log.LogTypeKey = EventLogController.EventLogType.HOST_ALERT.ToString ();
                log.LogPortalID = portalId;
                log.AddProperty ("Message", $"Cannot execute {actionKey} action");
                EventLogController.Instance.AddLog (log);
            }
        }

        public void ExecuteDiscussAction (string actionKey, int entryId, int portalId, int userId)
        {
            lock (discussLock) {
                var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId);
                if (newsEntry != null && string.IsNullOrEmpty (newsEntry.DiscussProviderKey)) {
                    var discussProvider = GetDiscussProviderByKey (actionKey);
                    if (discussProvider != null) {
                        var discussEntryId = discussProvider.Discuss (newsEntry, portalId, userId);
                        if (!string.IsNullOrEmpty (discussEntryId)) {
                            newsEntry.DiscussProviderKey = discussProvider.ProviderKey;
                            newsEntry.DiscussEntryId = discussEntryId;
                            NewsRepository.Instance.UpdateNewsEntry (newsEntry);
                            RedirectToDiscussion (newsEntry, discussProvider);
                        }
                        else {
                            LogAdminAlert ($"Error adding discussion for news entry using {actionKey} provider", portalId);
                        }
                    }
                    else {
                        LogAdminAlert ($"Cannot add discussion for news entry, {actionKey} provider does not exists", portalId);
                    }
                }
            }
        }

        public void ExecuteJoinDiscussionAction (int entryId, int portalId)
        {
            var newsEntry = NewsRepository.Instance.GetNewsEntry (entryId, portalId);
            if (newsEntry != null && !string.IsNullOrEmpty (newsEntry.DiscussProviderKey)) {
                var discussProvider = GetDiscussProviderByKey (newsEntry.DiscussProviderKey);
                if (discussProvider != null) {
                    RedirectToDiscussion (newsEntry, discussProvider);
                } else {
                    LogAdminAlert ($"Cannot redirect to discussion, {newsEntry.DiscussProviderKey} provider does not exists", portalId);
                }
            }
        }

        protected IDiscussProvider GetDiscussProviderByKey (string providerKey)
        {
            return NewsConfig.Instance.GetDiscussProviders ().FirstOrDefault (dp => dp.ProviderKey == providerKey);
        }

        protected void RedirectToDiscussion (INewsEntry newsEntry, IDiscussProvider discussProvider)
        {
            HttpContext.Current.Response.Redirect (discussProvider.GetDiscussUrl (newsEntry.DiscussEntryId), false);
        }

        protected void LogAdminAlert (string message, int portalId)
        {
            var log = new LogInfo ();
            log.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString ();
            log.LogPortalID = portalId;
            log.AddProperty ("Message", message);
            EventLogController.Instance.AddLog (log);
        }
    }
}

