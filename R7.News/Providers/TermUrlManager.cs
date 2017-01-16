//
//  TermUrlManager.cs
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
using DotNetNuke.Entities.Content.Taxonomy;
using R7.News.Components;

namespace R7.News.Providers
{
    public static class TermUrlManager
    {
        public static string GetUrl (int termId)
        {
            var termController = new TermController ();
            foreach (var provider in NewsConfig.Instance.GetTermUrlProviders ()) {
                var url = provider.GetUrl (termId, termController);
                if (!string.IsNullOrEmpty (url)) {
                    return url;
                }
            }

            return string.Empty;
        }

        public static string GetUrl (Term term)
        {
            foreach (var provider in NewsConfig.Instance.GetTermUrlProviders ()) {
                var url = provider.GetUrl (term);
                if (!string.IsNullOrEmpty (url)) {
                    return url;
                }
            }

            return string.Empty;
        }
    }
}
