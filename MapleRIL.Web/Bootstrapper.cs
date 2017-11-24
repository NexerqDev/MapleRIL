using MapleRIL.Common;
using MapleRIL.Web.Struct;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.Json;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web.Hosting;

namespace MapleRIL.Web
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        public Bootstrapper() : base()
        {
            // also init our goodies from here
            WebEngine.Init();
        }

        // serve statics from root
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Clear();
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/Static", "Static"));
            base.ConfigureConventions(nancyConventions);
        }

        // diags page
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                if (string.IsNullOrEmpty(WebEngine.Config.NancyAdminPassword))
                    return new DiagnosticsConfiguration();

                return new DiagnosticsConfiguration
                {
                    Password = WebEngine.Config.NancyAdminPassword
                };
            }
        }


        protected override IRootPathProvider RootPathProvider => new CustomRootPathProvider();

        public class CustomRootPathProvider : IRootPathProvider
        {
            public string GetRootPath()
            {
                return WebEngine.BasePath;
            }
        }
    }
}