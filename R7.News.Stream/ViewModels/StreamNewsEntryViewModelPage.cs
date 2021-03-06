//
//  StreamNewsEntryViewModelPage.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2016 Roman M. Yagodin
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
using System.Collections;
using System.Collections.Generic;

namespace R7.News.Stream.ViewModels
{
    public struct StreamNewsEntryViewModelPage
    {
        public IList<StreamNewsEntryViewModel> Page { get; private set; }

        public int TotalItems { get; private set; }

        public static StreamNewsEntryViewModelPage Empty
        {
            get { return new StreamNewsEntryViewModelPage (0, null); }
        }

        public StreamNewsEntryViewModelPage (int totalItems, IList<StreamNewsEntryViewModel> page)
        {
            TotalItems = totalItems;
            Page = page;
        }
    }
}
