using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Component.Downloader.Api.Wrappers
{
    public class ComponentWrapper : CustomItem
    {
        public ComponentWrapper(Item innerItem) : base(innerItem){}

        /// <summary>
        /// Returns the Datasource Template of a Component
        /// </summary>
        public TemplateItem DatasourceTemplate
        {
            get
            {
                ReferenceField field = InnerItem.Fields["Datasource Template"];
                return field == null ? null : field.TargetItem;
            }
        }
    }
}
