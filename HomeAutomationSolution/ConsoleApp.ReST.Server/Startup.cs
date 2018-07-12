using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System.Web.Http;

namespace ConsoleApp.ReST.Server
{
    public class Startup
    {
        // This method is required by Katana:
        public void Configuration(IAppBuilder app)
        {
            var webApiConfiguration = ConfigureWebApi();
            app.UseWebApi(webApiConfiguration);

            var physicalFileSystem = new PhysicalFileSystem(@"./pages");
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = physicalFileSystem
            };
            options.StaticFileOptions.FileSystem = physicalFileSystem;
            options.StaticFileOptions.ServeUnknownFileTypes = true;
            options.DefaultFilesOptions.DefaultFileNames = new[]
            {
                "DeviceControl.html"
            };

            app.UseFileServer(options);
        }


        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            return config;
        }
    }
}
