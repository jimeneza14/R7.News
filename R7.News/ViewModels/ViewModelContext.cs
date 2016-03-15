﻿//
//  ViewModelContext.cs
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
using System.Web.UI;
using DotNetNuke.R7;
using DotNetNuke.UI.Modules;

namespace R7.News.ViewModels
{
    public class ViewModelContext<TSettings>: ViewModelContext
        where TSettings: SettingsWrapper, new ()
    {
        public ViewModelContext (IModuleControl module): base (module)
        {}

        public ViewModelContext (Control control, IModuleControl module): base (control, module)
        {}

        public ViewModelContext (IModuleControl module, TSettings settings): base (module)
        {
            this.settings = settings;
        }

        public ViewModelContext (Control control, IModuleControl module, TSettings settings): base (control, module)
        {
            this.settings = settings;
        }

        private TSettings settings;
        public TSettings Settings
        {
            get
            {
                if (settings == null) {
                    settings = new TSettings ();
                    settings.Init (Module.ModuleId, Module.TabModuleId);
                }

                return settings;
            }
        }
    }
}

