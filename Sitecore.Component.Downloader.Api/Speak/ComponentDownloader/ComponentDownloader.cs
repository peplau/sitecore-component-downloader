using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Component.Downloader.Api.Managers;
using Sitecore.Component.Downloader.Api.Packages;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Component.Downloader.Api.Speak.ComponentDownloader
{
    public class ComponentDownloader : Web.PageCodes.PageCodeBase
    {
        #region Speak Controls at the page
        public Mvc.Presentation.Rendering LabComponentPath { get; set; }
        public Mvc.Presentation.Rendering TreeDsTemplate { get; set; }
        public Mvc.Presentation.Rendering TreeDsBaseTemplates { get; set; }
        public Mvc.Presentation.Rendering TxtDsTemplateEmpty { get; set; }
        public Mvc.Presentation.Rendering SectionDatasourceTemplate { get; set; }
        public Mvc.Presentation.Rendering TxtSelectedItems { get; set; }
        public Mvc.Presentation.Rendering TreeDsItems { get; set; }
        public Mvc.Presentation.Rendering TreePlaceholderSettings { get; set; }
        public Mvc.Presentation.Rendering LabPlaceholderSettings { get; set; }
        public Mvc.Presentation.Rendering TreeRules { get; set; }
        public Mvc.Presentation.Rendering LabRules { get; set; }
        public Mvc.Presentation.Rendering TreeExpEditorButtons { get; set; }
        public Mvc.Presentation.Rendering LabExpEditorButtons { get; set; }
        public Mvc.Presentation.Rendering TreeParametersTemplate { get; set; }
        public Mvc.Presentation.Rendering LabParametersTemplate { get; set; }
        public Mvc.Presentation.Rendering TreeThumbnail { get; set; }
        public Mvc.Presentation.Rendering LabThumbnail { get; set; }
        #endregion

        #region Helper Proterties
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
        #endregion

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
            var jsonPaths = HttpContext.Current.Request.Form["selectedPaths"];
            if (string.IsNullOrEmpty(jsonPaths))
                return;

            // Pack data
            var packData = new PackageData
            {
                PackageName = string.IsNullOrEmpty(ComponentItem.DisplayName)
                    ? ComponentItem.Name
                    : ComponentItem.DisplayName,
                Author = Context.User.GetLocalName()
            };

            // Sources and Paths
            var sourcesAndPaths = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonPaths);
            packData.Sources = new List<PackageSource>();
            foreach (var sourcesAndPath in sourcesAndPaths)
            {
                var pathsAndDbs = (List<dynamic>)sourcesAndPath.pathsAndDbs.ToObject<List<dynamic>>();
                if (!pathsAndDbs.Any())
                    continue;
                packData.Sources.Add(new PackageSource
                {
                    Name = sourcesAndPath.sourceName,
                    ItemEntries = pathsAndDbs.Select(p => new PackageItemEntry()
                    {
                        Database = p.database,
                        Path = p.path
                    }).ToList()
                });
            }

            // Add the rendering itself to the package
            packData.Sources.Add(new PackageSource
            {
                Name = packData.PackageName,
                ItemEntries = new List<PackageItemEntry>()
                {
                    new PackageItemEntry()
                    {
                        Database = ComponentItem.Database.Name,
                        Path = ComponentItem.Paths.Path
                    }
                }
            });

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

            BindDsTemplateAndItems();
            BindPlaceholderSettings();
            BindRules();
            BindExpEditorButtons();
            BindParametersTemplate();
            BindThumbnail();
        }

        private void BindThumbnail()
        {            
            var thumbnail = ComponentManager.GetThumbnail(ComponentItem);
            if (thumbnail != null)
            {
                TreeThumbnail.Parameters["RootItem"] = thumbnail.ID.ToString();
                TreeThumbnail.Parameters["Database"] = thumbnail.Database.Name;
            }
            else
            {
                TreeThumbnail.Parameters["IsVisible"] = "False";
                LabThumbnail.Parameters["IsVisible"] = "False";
            }
        }

        private void BindParametersTemplate()
        {
            var parameterTemplate = ComponentManager.GetParametersTemplate(ComponentItem);
            if (parameterTemplate != null)
            {
                TreeParametersTemplate.Parameters["RootItem"] = parameterTemplate.ID.ToString();
                TreeParametersTemplate.Parameters["Database"] = parameterTemplate.Database.Name;
            }
            else
            {
                TreeParametersTemplate.Parameters["IsVisible"] = "False";
                LabParametersTemplate.Parameters["IsVisible"] = "False";
            }
        }

        private void BindExpEditorButtons()
        {
            var buttons = ComponentManager.GetExperienceEditorButtons(ComponentItem);
            if (buttons != null && buttons.Any())
            {
                TreeExpEditorButtons.Parameters["RootItem"] = string.Join("|", buttons.Select(p => p.ID.ToString()).ToArray());
                TreeExpEditorButtons.Parameters["Database"] = buttons.First().Database.Name;
            }
            else
            {
                TreeExpEditorButtons.Parameters["IsVisible"] = "False";
                LabExpEditorButtons.Parameters["IsVisible"] = "False";
            }            
        }

        private void BindRules()
        {
            var ruleItems = LinkDatabaseManager.GetReferrersOfTemplates(ComponentItem, new[]{Constants.TemplateIds.RuleBase});
            if (ruleItems != null && ruleItems.Any())
            {
                TreeRules.Parameters["RootItem"] = string.Join("|", ruleItems.Select(p => p.ID.ToString()).ToArray());
                TreeRules.Parameters["Database"] = ComponentItem.Database.Name;
            }
            else
            {
                TreeRules.Parameters["IsVisible"] = "False";
                LabRules.Parameters["IsVisible"] = "False";
            }
        }

        private void BindPlaceholderSettings()
        {
            var placeholderSettingsItems = LinkDatabaseManager.GetReferrersOfTemplates(ComponentItem, new[] { Constants.TemplateIds.PlaceholderSettings });
            if (placeholderSettingsItems != null && placeholderSettingsItems.Any())
            {
                TreePlaceholderSettings.Parameters["RootItem"] = string.Join("|",placeholderSettingsItems.Select(p => p.ID.ToString()).ToArray());
                TreePlaceholderSettings.Parameters["Database"] = ComponentItem.Database.Name;
            }
            else
            {
                TreePlaceholderSettings.Parameters["IsVisible"] = "False";
                LabPlaceholderSettings.Parameters["IsVisible"] = "False";
            }
        }

        private void BindDsTemplateAndItems()
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
            // Base Templates of Datasource Templates
            TreeDsBaseTemplates.Parameters["RootItem"] = string.Join("|",
                DatasourceTemplate.BaseTemplates.Select(p => p.ID.ToString()).ToArray());
            TreeDsBaseTemplates.Parameters["Database"] = DatasourceTemplate.Database.Name;

            // Items - of the same type + Datasource Items
            var repositoryItems = ComponentManager.GetDatasourceLocations(ComponentItem);
            var itemsOfDsTemplate = LinkDatabaseManager.GetReferrersOfTemplates(DatasourceTemplate, new[] { DatasourceTemplate.ID });
            var items = new List<Item>();
            items.AddRange(repositoryItems);
            items.AddRange(itemsOfDsTemplate.Where(item => !repositoryItems.Any(p => item.Axes.IsDescendantOf(p))));
            if (!items.Any()) 
                return;

            TreeDsItems.Parameters["RootItem"] = string.Join("|",items.Select(p => p.ID.ToString()).ToArray());
            TreeDsItems.Parameters["Database"] = DatasourceTemplate.Database.Name;
        }
    }
}
