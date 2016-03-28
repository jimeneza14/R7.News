﻿//
//  StreamViewModel.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Modules;
using R7.News.Components;
using R7.News.Data;
using R7.News.Models;
using R7.News.Stream.Components;
using R7.News.ViewModels;

namespace R7.News.Stream.ViewModels
{
    public class StreamViewModel: ViewModelContext<StreamSettings>
    {
        public StreamViewModel (IModuleControl module, StreamSettings settings): base (module, settings)
        {
        }

        public StreamModuleNewsEntryViewModelPage GetPage(int pageIndex, int pageSize)
        {
            if (pageIndex == 0 && pageSize == Settings.PageSize) {
                var cacheKey = NewsRepository.NewsCacheKeyPrefix + "ModuleId=" + Module.ModuleId + "&PageIndex=0&PageSize=" + pageSize;
                return DataCache.GetCachedData<StreamModuleNewsEntryViewModelPage> (
                    new CacheItemArgs (cacheKey, NewsConfig.Instance.DataCacheTime, CacheItemPriority.Normal),
                    c => GetPageInternal (pageIndex, pageSize)
                );
            }

            return GetPageInternal (pageIndex, pageSize);
        }

        protected StreamModuleNewsEntryViewModelPage GetPageInternal (int pageIndex, int pageSize)
        {
            IEnumerable<ModuleNewsEntryInfo> items;

            if (Settings.ShowAllNews) {
                items = NewsRepository.Instance.GetModuleNewsEntries (Module.ModuleId, Module.PortalId);
            }
            else {
                items = NewsRepository.Instance.GetModuleNewsEntriesByTerms (Module.ModuleId, 
                    Module.PortalId, Settings.IncludeTerms);
            }

            // TODO: Implement check for pageIndex > totalPages

            if (pageIndex < 0 || items == null || !items.Any ()) {

                return new StreamModuleNewsEntryViewModelPage {
                    TotalItems = 0,
                    Page = null
                };
            }

            return new StreamModuleNewsEntryViewModelPage {
                TotalItems = items.Count (),
                Page = items.OrderByDescending (ne => ne.PublishedOnDate ())
                    .Skip (pageIndex * pageSize)
                    .Take (pageSize)
                    .Select (ne => new StreamModuleNewsEntryViewModel (ne, this))
                    .ToList ()
                };
        }

    }
}

