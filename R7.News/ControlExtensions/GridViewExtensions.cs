﻿//
//  GridViewExtensions.cs
//
//  Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
//  Copyright (c) 2015, 2016 Roman M. Yagodin
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
using System.Web.UI.WebControls;
using DotNetNuke.Services.Localization;

namespace R7.News.ControlExtensions
{
    // TODO: Move to base library
    public static class GridViewExtensions
    {
        public static void LocalizeColumnHeaders (this GridView gv, string suffix, string resourceFile)
        {
            foreach (DataControlField column in gv.Columns)
                column.HeaderText = Localization.GetString (column.HeaderText + suffix, resourceFile);
        }
    }
}