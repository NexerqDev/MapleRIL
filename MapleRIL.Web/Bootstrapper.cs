using Nancy;
using Nancy.Conventions;
using Nancy.Diagnostics;

namespace MapleRIL.Web
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

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
                if (string.IsNullOrEmpty(Program.Config.NancyAdminPassword))
                    return new DiagnosticsConfiguration();

                return new DiagnosticsConfiguration
                {
                    Password = Program.Config.NancyAdminPassword
                };
            }
        }
    }
}