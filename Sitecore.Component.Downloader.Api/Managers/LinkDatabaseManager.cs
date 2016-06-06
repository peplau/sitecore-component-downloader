using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Data;

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
        /// <param name="referredItem"></param>
        /// <param name="templateIds"></param>
        /// <param name="includeStandardValues"></param>
        /// <returns></returns>
        public static Item[] GetReferrersOfTemplates(Item referredItem, ID[] templateIds, bool includeStandardValues = false)
        {
            var links = Globals.LinkDatabase.GetReferrers(referredItem);
            if (links == null)
                return new Item[0];

            //var linkedItems = links.Select(i => i.GetSourceItem())
            //    .Where(
            //        i =>
            //            i != null &&
            //            (i.TemplateID == template.ID || i.Template.BaseTemplates.Any(t => t.ID == template.ID)));

            var linkedItems = links.Select(i => i.GetSourceItem())
                .Where(
                    i =>
                        i != null &&
                        (templateIds.Any(t => t == i.TemplateID) || i.Template.BaseTemplates.Select(t=>t.ID).Intersect(templateIds).Any()));

            if (!includeStandardValues)
                linkedItems = linkedItems.Where(i => !i.Name.Equals("__standard values", StringComparison.InvariantCultureIgnoreCase));

            return linkedItems.ToArray();
        }
    }
}