using Sitecore.Data;

namespace Sitecore.Component.Downloader.Api
{
    public static class Constants
    {
        public struct Settings
        {
            public const string DashboardPath = "SCD.DashboardPath";
        }

        public struct QueryStringParams
        {
            public const string ComponentId = "cid";
        }

        public struct TemplateIds
        {
            public static readonly ID Sublayout = new ID("{0A98E368-CDB9-4E1E-927C-8E0C24A003FB}");
            public static readonly ID Rendering = new ID("{92D4A8C4-5754-4E1A-96A6-095BD193E12B}");
            public static readonly ID ControllerRendering = new ID("{2A3E91A0-7987-44B5-AB34-35C2D9DE83B9}");
            public static readonly ID ViewRendering = new ID("{99F8905D-4A87-4EB8-9F8B-A9BEBFB3ADD6}");
            public static readonly ID PlaceholderSettings = new ID("{5C547D4E-7111-4995-95B0-6B561751BF2E}");

            public static readonly ID RuleBase = new ID("{34E1F0D9-CCFF-4F03-A38D-933437AA3A2A}");            
            public static readonly ID Rule = new ID("{D9BDF22F-6E17-47F3-AB64-49C717BA92DA}");
            public static readonly ID InsertOptionsRule = new ID("{664E5035-EB8C-4BA1-9731-A098FCC9127A}");
            public static readonly ID ValidationRule = new ID("{0512BDE9-5696-42C4-8C7D-B349EDA9CEF9}");
            public static readonly ID ContentEditorWarningRule = new ID("{71A2C881-FBB3-4A23-A187-7FD50A20F924}");
            public static readonly ID ConditionalRenderingRule = new ID("{550B5CEF-242C-463F-8ED5-983922A39863}");            
        }
    }
}