using System;
using System.Linq;
using System.Web;
using Sitecore.Component.Downloader.Api.Wrappers;
using Sitecore.Data;

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
        #endregion

        private ComponentWrapper _componentItem;
        public ComponentWrapper ComponentItem
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
                _componentItem = new ComponentWrapper(item);
                return _componentItem;
            }
        }

        public override void Initialize()
        {
            //		HttpContext.Current.Request.HttpMethod	"POST"	string
            //		HttpContext.Current.Request.Form["selectedPaths"]	"/sitecore/templates/Keystone/Components/Accordion Container|/sitecore/templates/Keystone/Components/Accordion Container/Teste|/sitecore/templates/Keystone/Components/Accordion Container/Teste/Campo 1|/sitecore/templates/Keystone/Components/Accordion Container/Teste/Campo 2|/sitecore/templates/Keystone/Components/Accordion Container/__Standard Values|/sitecore/templates/System/Templates/Standard template|/sitecore/templates/System/Templates/Standard template|/sitecore/templates/Keystone/Base/Fields/Base - Component/__Standard Values|/sitecore/templates/Keystone/Base/Fields/Base - Component/Styles|/sitecore/templates/Keystone/Base/Fields/Base - Component/Styles/Additional Styles|/sitecore/templates/Keystone/Base/Fields/Base - Component/Custom|/sitecore/templates/Keystone/Base/Fields/Base - Component/Custom/Custom Styles|/sitecore/templates/Keystone/Base/Fields/Base - Component/Mobile|/sitecore/templates/Keystone/Base/Fields/Base - Component/Mobile/ResponsiveStyles|/sitecore/templates/System/Templates/Standard template|/sitecore/templates/Keystone/Base/Fields/Components/Base - Accordion Container/Accordion Container Fields|/sitecore/templates/Keystone/Base/Fields/Components/Base - Accordion Container/Accordion Container Fields/Allow Multiple Panels Opened|"	string
            if (HttpContext.Current.Request.HttpMethod == "POST" 
                && HttpContext.Current.Request.Form.AllKeys.Contains("selectedPaths"))
            {
                DownloadPackage();
                return;
            }

            // Escape if Component can't be found
            if (ComponentItem == null)
                return;

            BindControls();
        }

        private void DownloadPackage()
        {
            throw new NotImplementedException();
        }

        private void BindControls()
        {
            LabComponentPath.Parameters["Text"] = ComponentItem.InnerItem.Paths.Path;

            BindDsTemplate();
        }

        private void BindDsTemplate()
        {
            if (ComponentItem.DatasourceTemplate == null)
            {
                TxtDsTemplateEmpty.Parameters["IsVisible"] = "True";
                SectionDatasourceTemplate.Parameters["IsVisible"] = "False";
                return;
            }

            // Datasource Template
            TreeDsTemplate.Parameters["RootItem"] = ComponentItem.DatasourceTemplate.ID.ToString();
            TreeDsTemplate.Parameters["Database"] = ComponentItem.DatasourceTemplate.Database.Name;

            // Base Templates
            TreeDsBaseTemplates.Parameters["RootItem"] = String.Join("|",
                ComponentItem.DatasourceTemplate.BaseTemplates.Select(p => p.ID.ToString()).ToArray());
            TreeDsBaseTemplates.Parameters["Database"] = ComponentItem.DatasourceTemplate.Database.Name;
        }
    }
}
