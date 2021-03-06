using System.IO;
using System.Linq;
using System.Reflection;
using Baseline;
using StoryTeller.Remotes.Messaging;
using StoryTeller.Results;
using StoryTeller.Util;

namespace ST.Client
{
    public static class HomeEndpoint
    {
        public static HtmlDocument BuildPage(IApplication application, OpenInput input)
        {
            var document = new HtmlDocument {Title = "Storyteller 4"};


            writeInitialDataIntoPage(document, application);

            document.Add("div").Id("header-container");
            document.Add("div").Id("body-pane").AddClass("container");
            document.Add("div").Id("main");

#if DEBUG
            WriteClientAssetsDebugMode(document, input.DevFlag);
#else

            writeClientSideAssetsFromEmbeds(document);
#endif


            return document;
        }

        private static void writeClientSideAssetsFromEmbeds(HtmlDocument document)
        {
            BatchResultsWriter.WriteCSS(document);
            document.Head.Add("link")
                .Attr("rel", "stylesheet")
                .Attr("href", "//maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css");


            var bundleJS = typeof(HomeEndpoint).GetTypeInfo().Assembly
                .GetManifestResourceStream("StorytellerRunner.bundle.js").ReadAllText();

            var scriptTag = new HtmlTag("script").Attr("type", "text/javascript").Text(bundleJS).Encoded(false);
            document.Body.Append(scriptTag);
        }

        public static void WriteClientAssetsDebugMode(HtmlDocument document, bool devMode, string bundleName = "/bundle.js")
        {
            var stylesheets = new[] {"bootstrap.min.css", "storyteller.css", "font-awesome.min.css", "fixed-data-table.min.css"};
            var tags = stylesheets.Select(file =>
            {
                var path = $"/public/stylesheets/{file}";
                return new HtmlTag("link").Attr("rel", "stylesheet").Attr("href", path);
            });

            document.Head.Append(tags);

            var bundleUrl = devMode ? "http://localhost:3001/client/public/javascript/bundle.js" : bundleName;
            var scriptTag = new HtmlTag("script").Attr("type", "text/javascript").Attr("src", bundleUrl);
            document.Body.Append(scriptTag);
        }

        private static void writeInitialDataIntoPage(HtmlDocument document, IApplication application)
        {
            var cleanJson = JsonSerialization.ToCleanJson(application.Persistence.Hierarchy.Top);
            document.Body.Add("div").Hide().Id("hierarchy-data").Text(cleanJson);

            var resultJson = JsonSerialization.ToCleanJson(application.Persistence.AllCachedResults());
            document.Body.Add("div").Hide().Id("result-data").Text(resultJson);

            var model = application.BuildInitialModel();

            var script = new StringWriter();
            script.WriteLine();
            script.WriteLine($"var Storyteller = {{wsAddress: '{application.Client.WebSocketsAddress}'}};");
            script.WriteLine();
            script.WriteLine("Storyteller.initialization = {0};",
                JsonSerialization.ToCleanJson(model));
            script.WriteLine();


            document.Head.Add("script").Encoded(false).Text(script.ToString()).Attr("type", "text/javascript");
        }
    }
}