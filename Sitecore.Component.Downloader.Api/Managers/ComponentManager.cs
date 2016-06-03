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
    }
}
