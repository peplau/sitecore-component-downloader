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
        /// <param name="referredItem"></param>
        /// <returns></returns>
        public static Item[] GetReferrersOfItem(Item referredItem)
        {
            var links = Globals.LinkDatabase.GetReferrers(referredItem);
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
        public static Item[] GetReferrersOfTemplate(Item referredItem, TemplateItem template)
        {
            var links = Globals.LinkDatabase.GetReferrers(referredItem);
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