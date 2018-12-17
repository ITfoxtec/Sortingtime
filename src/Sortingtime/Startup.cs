using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sortingtime.Models;
using Sortingtime.Infrastructure.Configuration;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;

namespace Sortingtime
{
    public class Startup
    {
        public static IHostingEnvironment Environment { get; set; }
        public static IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            Environment = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
#if DEBUG
                b => b.MigrationsAssembly("Sortingtime")
#endif                
                )); 

            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>  {
                o.Password.RequiredLength = 5;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddSortingtime();

            services.AddSortingtimeAddDataProtection(Configuration);

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IAntiforgery antiforgery, IServiceProvider serviceProvider)
        {
            loggerFactory.AddApplicationInsights(serviceProvider);

            app.UseExceptionHandler("/E/Error");

            if (env.IsDevelopment())
            {
                app.UseStaticFiles();
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = (context) =>
                    {
                        var headers = context.Context.Response.GetTypedHeaders();
                        headers.CacheControl = new CacheControlHeaderValue()
                        {
                            MaxAge = TimeSpan.FromDays(365),
                        };
                    }

                });
            }

            app.UseAuthentication();
           
            app.UseSecureJSNLog(loggerFactory);

            app.Use(next => context =>
            {
                if (context.Request.Path.Value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { Secure = true });
                }

                return next(context);
            });

            app.UseLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=E}/{action=Index}/{id?}");
            });
        }
        
    }
}
