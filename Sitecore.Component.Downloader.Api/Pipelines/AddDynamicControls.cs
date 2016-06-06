using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Pipelines.InsertRenderings;

namespace Sitecore.Component.Downloader.Api.Pipelines
{
    public class AddDynamicControls : InsertRenderingsProcessor
    {
        public override void Process(InsertRenderingsArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.ContextItem == null)
                return;
            var device = Context.Device;
            if (device == null)
                return;




            //foreach (Item obj in args.ContextItem.Axes.GetAncestors())
            //{
            //    var renderings = obj.Visualization.GetRenderings(device, true);
            //    args.Renderings.AddRange(Enumerable.Where<RenderingReference>((IEnumerable<RenderingReference>)renderings, (Func<RenderingReference, bool>)(rendering => RenderingSettingsExt.Cascader(rendering.Settings))));
            //}
        }
    }
}
