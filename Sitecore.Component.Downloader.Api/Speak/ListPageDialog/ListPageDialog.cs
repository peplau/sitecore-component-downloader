using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Component.Downloader.Api.Managers;
using Sitecore.Component.Downloader.Api.Packages;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Component.Downloader.Api.Speak.ListPageDialog
{
    class ListPageDialog : Web.PageCodes.PageCodeBase
    {
        #region Speak Controls at the page
        public Mvc.Presentation.Rendering LabComponentPath { get; set; }
        public Mvc.Presentation.Rendering TreeDsTemplate { get; set; }
        public Mvc.Presentation.Rendering TreeDsBaseTemplates { get; set; }
        public Mvc.Presentation.Rendering TxtDsTemplateEmpty { get; set; }
        public Mvc.Presentation.Rendering SectionDatasourceTemplate { get; set; }
        public Mvc.Presentation.Rendering TxtSelectedItems { get; set; }
        public Mvc.Presentation.Rendering TreeDsItems { get; set; }
        #endregion

        private Item _componentItem;
        public Item ComponentItem
        {
            get
            {
                if (_componentItem != null)
                    return _componentItem;

                var strId = HttpContext.Current.Request.QueryString[Constants.QueryStringParams.ComponentId];
                if (string.IsNullOrEmpty(strId))
                    return null;

                ID id;
                if (!ID.TryParse(strId,out id))
                    return null;

                var item = Context.ContentDatabase.GetItem(id);
                if (item == null)
                    return null;
                _componentItem = item;
                return _componentItem;
            }
        }

        private TemplateItem _datasourceTemplate;
        public TemplateItem DatasourceTemplate
        {
            get
            {
                if (_datasourceTemplate != null)
                    return _datasourceTemplate;
                _datasourceTemplate = ComponentManager.GetDatasourceTemplate(ComponentItem);
                return _datasourceTemplate;
            }
        }

        public override void Initialize()
        {
            // Escape if Component can't be found
            if (ComponentItem == null)
                return;

            // Create package and download it
            if (HttpContext.Current.Request.HttpMethod == "POST" 
                && HttpContext.Current.Request.Form.AllKeys.Contains("selectedPaths"))
            {
                DownloadPackage();
                return;
            }

            BindControls();
        }

        private void DownloadPackage()
        {
            var strPath = HttpContext.Current.Request.Form["selectedPaths"];
            if (string.IsNullOrEmpty(strPath))
                return;

            // Pack data
            var packData = new PackageData
            {
                Database = ComponentItem.Database.Name,
                PackageName = string.IsNullOrEmpty(ComponentItem.DisplayName)
                    ? ComponentItem.Name
                    : ComponentItem.DisplayName,
                Author = Context.User.GetLocalName()
            };

            // Paths
            var paths = strPath.Split('|').Where(p=>!string.IsNullOrEmpty(p)).ToList();
            var source = new PackageSource { Name = packData.PackageName, Paths = paths };
            packData.Sources = new List<PackageSource> { source };

            // Create Pack
            var dataFolder = Configuration.Settings.PackagePath;
            if (!dataFolder.EndsWith("/"))
                dataFolder += "/";

            // Check if file already exists
            var packPath = string.Format("{0}{1}{2}.zip", dataFolder, packData.PackageName, "");
            var counter = 1;
            while (File.Exists(packPath))
            {
                counter++;
                packPath = string.Format("{0}{1}{2}.zip", dataFolder, packData.PackageName, counter);
            }

            string error;
            var success = PackageManager.CreatePackage(packData, packPath, out error);

            // Download Pack
            if (!success)
            {
                Diagnostics.Log.Error("Error creating package - " + error, this);
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl);
                return;
            }

            HttpContext.Current.Response.Redirect(packPath);
        }

        private void BindControls()
        {
            LabComponentPath.Parameters["Text"] = ComponentItem.Paths.Path;

            BindDsTemplate();
        }

        private void BindDsTemplate()
        {
            if (DatasourceTemplate == null)
            {
                TxtDsTemplateEmpty.Parameters["IsVisible"] = "True";
                SectionDatasourceTemplate.Parameters["IsVisible"] = "False";
                return;
            }

            // Datasource Template
            TreeDsTemplate.Parameters["RootItem"] = DatasourceTemplate.ID.ToString();
            TreeDsTemplate.Parameters["Database"] = DatasourceTemplate.Database.Name;

            // Base Templates
            TreeDsBaseTemplates.Parameters["RootItem"] = string.Join("|",
                DatasourceTemplate.BaseTemplates.Select(p => p.ID.ToString()).ToArray());
            TreeDsBaseTemplates.Parameters["Database"] = DatasourceTemplate.Database.Name;

            // Items of the same type
            var items = LinkDatabaseManager.GetItemsOfTemplate(DatasourceTemplate);
            if (items != null && items.Any())
            {
                TreeDsItems.Parameters["RootItem"] = string.Join("|",items.Select(p => p.ID.ToString()).ToArray());
                TreeDsItems.Parameters["Database"] = DatasourceTemplate.Database.Name;
            }
        }
    }
}
