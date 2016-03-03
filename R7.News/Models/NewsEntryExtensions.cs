﻿//
//  NewsEntityExtensions.cs
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
using System.Linq;
using System.Collections.Generic;
using DotNetNuke.Entities.Content;
using R7.News.Components;
using R7.News.Models.Data;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;

namespace R7.News.Models
{
    public static class NewsEntryExtensions
    {
        public static INewsEntry WithContentItem (this INewsEntry newsEntry)
        {
            newsEntry.ContentItem = NewsDataProvider.Instance.ContentController.GetContentItem (newsEntry.ContentItemId);
            return newsEntry;
        }

        public static IEnumerable<INewsEntry> WithContentItems (this IEnumerable<INewsEntry> newsEntries)
        {
            var contentController = new ContentController ();
            var contentItems = contentController.GetContentItemsByContentType (NewsDataProvider.Instance.NewsContentType);

            return newsEntries.Join (contentItems.DefaultIfEmpty (), 
                ne => ne.ContentItemId,
                ci => ci.ContentItemId, 
                (ne, ci) => { 
                    ne.ContentItem = ci; 
                    return ne; 
                }
            );
        }

        public static IEnumerable<INewsEntry> WithContentItemsOneByOne (this IEnumerable<INewsEntry> newsEntries)
        {
            foreach (var newsEntry in newsEntries) {
                yield return newsEntry.WithContentItem ();
            }
        }

        public static INewsEntry WithNewsSource (this INewsEntry newsEntry)
        {
            newsEntry.Source = NewsSourceRepository.Instance.GetSource (newsEntry.SourceId, newsEntry.SourceItemId);
            return newsEntry;
        }

        public static IEnumerable<INewsEntry> WithNewsSources (this IEnumerable<INewsEntry> newsEntries)
        {
            foreach (var newsEntry in newsEntries) {
                yield return newsEntry.WithNewsSource ();
            }
        }

        public static IFileInfo GetImage (this INewsEntry newsEntry)
        {
            return newsEntry.ContentItem.Images.FirstOrDefault ();
        }

        public static string GetImageUrl (this INewsEntry newsEntry, int width)
        {
            var image = newsEntry.GetImage ();
            if (image != null) {
                return Globals.AddHTTP (PortalSettings.Current.PortalAlias.HTTPAlias)
                    + "/imagehandler.ashx?fileticket=" + UrlUtils.EncryptParameter (image.FileId.ToString ()) 
                    + "&width=" + width;
            }

            return string.Empty;
        }
    }
}

