using System;
using System.Collections.Specialized;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Component.Downloader.Api.Commands
{
    [Serializable]
    public class Downloader : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            Context.ClientPage.Start(this, "Run",
                new ClientPipelineArgs(new NameValueCollection()
                {
                    {Constants.QueryStringParams.ComponentId, context.Items[0].ID.ToString()}
                }));
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                    ;
            }
            else
            {
                var path =
                    Configuration.Settings.GetSetting(Constants.Settings.DashboardPath);
                var urlString =
                    new UrlString(UIUtil.GetUri(path,
                        Constants.QueryStringParams.ComponentId + "=" + args.Parameters[Constants.QueryStringParams.ComponentId]));

                SheerResponse.ShowModalDialog(urlString.ToString(), "600px", "600px", "", true);
                args.WaitForPostBack();
            }
        }

        /// <summary>
        /// Queries the state of the command.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The state of the command.
        /// </returns>
        /// <contract><requires name="context" condition="not null"/></contract>
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length != 1)
                return CommandState.Hidden;
            var item = context.Items[0];
            if (item.TemplateID != Constants.TemplateIds.ControllerRendering 
                && item.TemplateID != Constants.TemplateIds.Rendering 
                && item.TemplateID != Constants.TemplateIds.Sublayout 
                && item.TemplateID != Constants.TemplateIds.ViewRendering)
                return CommandState.Hidden;
            return base.QueryState(context);
        }
    }
}
