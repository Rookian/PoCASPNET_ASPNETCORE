using System;
using System.Threading;
using Lib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Disposal;
using WebCore.MissingDI;

namespace WebCore
{
    public class Startup
    {
        private readonly AsyncLocal<Scope> _scopeProvider = new AsyncLocal<Scope>();
        private readonly IKernel _kernel = new StandardKernel();

        private object Resolve(Type type) => _kernel.Get(type);
        private Scope RequestScope(IContext context) => _scopeProvider.Value;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRequestScopingMiddleware(() => _scopeProvider.Value = new Scope());
            services.AddCustomControllerActivation(Resolve);
            services.AddCustomViewComponentActivation(Resolve);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            RegisterApplicationComponents(app, loggerFactory);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        
        private void RegisterApplicationComponents(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {

            // Register application services
            _kernel.Bind(app.GetControllerTypes()).ToSelf().InScope(RequestScope);

            // Cross-wire required framework services
            _kernel.BindToMethod(app.GetRequestService<IViewBufferScope>);
            _kernel.Bind<ILoggerFactory>().ToConstant(loggerFactory);

            _kernel.Bind<IServiceA>().To<ServiceA>().InScope(RequestScope);
        }

        private sealed class Scope : DisposableObject { }
    }

    public static class BindingHelpers
    {
        public static void BindToMethod<T>(this IKernel config, Func<T> method) => config.Bind<T>().ToMethod(c => method());
    }
}
