//
//  TermLinks.ascx.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Entities.Modules;
using R7.DotNetNuke.Extensions.ViewModels;
using R7.News.Controls.ViewModels;

namespace R7.News.Controls
{
    public class TermLinks: UserControl
    {
        #region Controls

        protected ListView listTermLinks;

        #endregion

        #region Public properties

        public string CssClass { get; set; }

        public List<Term> DataSource { get; set; }

        public PortalModuleBase Module { get; set; }

        #endregion

        private ViewModelContext viewModelContext;

        protected ViewModelContext ViewModelContext
        {
            get { return viewModelContext ?? (viewModelContext = new ViewModelContext (this, Module)); }
        }

        public override void DataBind ()
        {
            if (DataSource != null && Module != null) {
                listTermLinks.DataSource = DataSource.Select (t => new TermLinksViewModel (t, ViewModelContext));
                listTermLinks.DataBind ();
            }

            base.DataBind ();
        }
    }
}

