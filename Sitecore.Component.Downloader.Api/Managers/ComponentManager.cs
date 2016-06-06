using Sitecore.Data.Fields;
using Sitecore.Data.Items;

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
            var contextPath = "/sitecore/content";
            var args = new Sitecore.Pipelines.GetRenderingDatasource.GetRenderingDatasourceArgs(component)
            {
                ContextItemPath = contextPath
            };

            var pipelines = Sitecore.Pipelines.CorePipelineFactory.GetPipeline("getRenderingDatasource",string.Empty);
            pipelines.Run(args);

            // After processing it will fill args.DatasourceRoots
            return args.DatasourceRoots.ToArray();
        }
    }
}
