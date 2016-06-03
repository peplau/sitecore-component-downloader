using System;
using System.Linq;
using Sitecore.Data.Items;

namespace Sitecore.Component.Downloader.Api.Managers
{
    public static class LinkDatabaseManager
    {
        /// <summary>
        /// Get all Items linking to this item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Item[] GetReferrersAsItems(Item item)
        {
            var links = Globals.LinkDatabase.GetReferrers(item);
            if (links == null)
                return new Item[0];
            var linkedItems = links.Select(i => i.GetSourceItem()).Where(i => i != null);
            return linkedItems.ToArray();
        }

        /// <summary>
        /// Get items of the given template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Item[] GetItemsOfTemplate(TemplateItem template)
        {
            var links = Globals.LinkDatabase.GetReferrers(template);
            if (links == null)
                return new Item[0];
            var linkedItems = links.Select(i => i.GetSourceItem())
                .Where(
                    i =>
                        i != null &&
                        (i.TemplateID == template.ID || i.Template.BaseTemplates.Any(t => t.ID == template.ID)));
            return linkedItems.ToArray();
        }
    }
}