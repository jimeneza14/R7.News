using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.UI.Modules;
using R7.Dnn.Extensions.ViewModels;
using R7.Dnn.Extensions.Controls;
using R7.News.Components;
using R7.News.Models;
using R7.News.Controls.Models;

namespace R7.News.Controls
{
    // TODO: Rename to EditActionsControl or just EditActions?
    public class ActionsControl: UserControl
    {
        public int EntryId { get; set; }

        public int? EntryTextId { get; set; }

        ViewModelContext dnnContext;
        protected ViewModelContext DnnContext {
            get { return dnnContext ?? (dnnContext = new ViewModelContext (this, this.FindParentOfType<IModuleControl> ())); }
        }

        public ActionButtons ActionButtons => (ActionButtons) FindControl ("actionButtons");

        public bool ShowSyncTabAction { get; set; }

        public bool ShowDuplicateAction { get; set; }

        public bool ShowLoadMoreTextAction { get; set; }

        protected string LocalizeString (string text)
        {
            return DnnContext.LocalizeString (text);
        }

        protected string EditUrl ()
        {
            return DnnContext.Module.EditUrl ("entryid", EntryId.ToString (), "EditNewsEntry");
        }

        protected NewsEntryAction DuplicateAction => new NewsEntryAction {
            EntryId = EntryId,
            Action = NewsEntryActions.Duplicate,
            ModuleId = DnnContext.Module.ModuleId,
            Enabled = true,
            Params = new string [] { DnnContext.Module.ModuleId.ToString () }
        };

        protected NewsEntryAction SyncTabAction => new NewsEntryAction {
            EntryId = EntryId,
            Action = NewsEntryActions.SyncTab,
            ModuleId = DnnContext.Module.ModuleId,
            Enabled = true
        };

        protected bool IsEditable => DnnContext.Module.IsEditable;

        protected void btnExecuteAction_Command (object sender, CommandEventArgs e)
        {
            // Cannot use DnnContext here?
            var actionHandler = new ActionHandler ();
            var action = JsonExtensionsWeb.FromJson<NewsEntryAction> ((string) e.CommandArgument);
            actionHandler.ExecuteAction (action, PortalSettings.Current.PortalId, PortalSettings.Current.ActiveTab.TabID, action.ModuleId);
        }
    }
}

