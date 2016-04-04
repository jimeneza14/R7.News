﻿//
//  DataContext.cs
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
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.R7.Data;

namespace R7.News.Data
{
    public class NewsDataProvider: Dal2DataProvider
    {
        #region Singleton implementation

        private static readonly Lazy<NewsDataProvider> instance = new Lazy<NewsDataProvider> ();

        public static NewsDataProvider Instance
        {
            get { return instance.Value; }
        }

        #endregion

        #region Properties

        private ContentType newsContentType;
        public ContentType NewsContentType
        {
            get
            {
                if (newsContentType == null) {
                    var contentTypeController = new ContentTypeController ();
                    newsContentType = contentTypeController.GetContentTypes ()
                        .Where (ct => ct.ContentType == "R7_News_Entry")
                        .SingleOrDefault ();
                }

                return newsContentType;
            }
        }

        private ContentController contentController;
        public ContentController ContentController
        {
            get
            { 
                if (contentController == null) {
                    contentController = new ContentController ();
                }

                return contentController;
            }
        }

        private ModuleController moduleController;
        public ModuleController ModuleController
        {
            get
            { 
                if (moduleController == null) {
                    moduleController = new ModuleController ();
                }

                return moduleController;
            }
        }

        #endregion
    }
}
