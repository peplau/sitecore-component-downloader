using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Component.Downloader.Api.Managers
{
    public static class ComponentManager
    {
        /// <summary>
        /// Returns the Datasource Template of a given Component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static TemplateItem GetDatasourceTemplate(Item component)
        {
            ReferenceField field = component.Fields["Datasource Template"];
            return field == null ? null : field.TargetItem;
        }

        /// <summary>
        /// Returns the items that are setup at field "Datasource Location" 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Item[] GetDatasourceLocations(Item component)
        {
            const string contextPath = "/sitecore/content";
            var args = new Sitecore.Pipelines.GetRenderingDatasource.GetRenderingDatasourceArgs(component)
            {
                ContextItemPath = contextPath
            };

            var pipelines = Sitecore.Pipelines.CorePipelineFactory.GetPipeline("getRenderingDatasource", string.Empty);
            pipelines.Run(args);

            // After processing it will fill args.DatasourceRoots
            return args.DatasourceRoots.ToArray();
        }

        /// <summary>
        /// Get Experience Editor Buttons for a given Rendering/Sublayout
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static Item[] GetExperienceEditorButtons(Item component)
        {
            MultilistField field = component.Fields["Page Editor Buttons"];
            return field == null ? null : field.GetItems();
        }

        /// <summary>
        /// Get Parameters Template for a given Rendering
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        internal static Item GetParametersTemplate(Item component)
        {
            ReferenceField field = component.Fields["Parameters Template"];
            return field == null ? null : field.TargetItem;
        }

        /// <summary>
        /// Get Thumbnail MediaItem
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        internal static Item GetThumbnail(Item component)
        {
            ThumbnailField field = component.Fields["__Thumbnail"];
            return field == null ? null : field.MediaItem;
        }
    }
}
