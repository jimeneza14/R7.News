﻿//
//  ModuleNewsEntryInfo.cs
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
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Web.Validators;

namespace R7.News.Models.Data
{
    [TableName ("r7_News")]
    [PrimaryKey ("EntryId", AutoIncrement = true)]
    [Scope ("PortalId")]
    public class ModuleNewsEntryInfo: IModuleNewsEntry
    {
        #region INewsEntry implementation

        public int EntryId { get; set; }

        public int PortalId { get; set; }

        public int ContentItemId { get; set; }

        public int? AgentModuleId { get; set; }

        public string Url { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? ThresholdDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int SortIndex { get; set; }

        public bool IsSticky { get; set; }

        public int? SourceId { get; set; }

        public int? SourceItemId { get; set; }

        [IgnoreColumn]
        public ContentItem ContentItem { get; set; }

        [IgnoreColumn]
        public ModuleInfo AgentModule { get; set; }

        [IgnoreColumn]
        public INewsSource Source { get; set; }

        [IgnoreColumn]
        public ICollection<INewsEntry> Group { get; set; }

        #endregion

        #region IModuleNewsEntry implementation

        [ReadOnlyColumn]
        public int? ModuleId { get; set; }

        [ReadOnlyColumn]
        public int? Visibility { get; set; }

        #endregion

        [IgnoreColumn]
        public NewsEntryVisibility NewsEntryVisibility
        {
            get { return this.GetNewsEntryVisibility (); }
            set 
            {
                if (value == NewsEntryVisibility.Show) {
                    Visibility = null;
                }
                else {
                    Visibility = (int?) value;
                }
            }
        }
    }
}
