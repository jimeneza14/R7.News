﻿//
//  StreamSettings.cs
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
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.R7;
using DotNetNuke.R7.Entities.Modules;

namespace R7.News.Stream.Components
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    public class StreamSettings : SettingsWrapper
    {
        public StreamSettings ()
        {
        }

        public StreamSettings (IModuleControl module) : base (module)
        {
        }

        public StreamSettings (ModuleInfo module) : base (module)
        {
        }

        #region Module settings

        public List<Term> IncludeTerms
        {
            get
            { 
                var termController = new TermController ();

                return ReadSetting<string> ("r7_News_Stream_IncludeTerms", string.Empty)
                    .Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select (ti => termController.GetTerm (int.Parse (ti)))
                    .ToList ();
            }

            set
            {
                WriteModuleSetting<string> ("r7_News_Stream_IncludeTerms", 
                    TextUtils.FormatList (";", value.Select (t => t.TermId)));
            }
        }

        public bool ShowAllNews
        {
            get { return ReadSetting<bool> ("r7_News_Stream_ShowAllNews", false); }
            set { WriteModuleSetting<bool> ("r7_News_Stream_ShowAllNews", value); }
        }

        #endregion

        #region Tab-specific module settings

        public int ThumbnailWidth
        {
            // TODO: Get default thumbnail width from config
            get { return ReadSetting<int> ("r7_News_Stream_ThumbnailWidth", 192); }
            set { WriteTabModuleSetting<int> ("r7_News_Stream_ThumbnailWidth", value); }
        }

        #endregion
    }
}

