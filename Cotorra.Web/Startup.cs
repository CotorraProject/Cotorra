using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CotorraNode.Common.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Cotorra.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        static Startup()
        {
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            if (Configuration["AzureAppConfigurationConnectionString"] != null)
            {
                builder.AddAzureAppConfiguration(opts =>
                {
                    opts.Connect(Configuration["AzureAppConfigurationConnectionString"])
                        .Select(KeyFilter.Any, labelFilter: "Platform")
                        .Select(KeyFilter.Any, labelFilter: "Cotorria")
                        .ConfigureRefresh(refresh =>
                        {
                            refresh.SetCacheExpiration(TimeSpan.FromHours(12));
                        });

                }
                , false);
            }

            builder.AddEnvironmentVariables();
            builder.AddJsonFile("appsettings.debug.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
            ConfigManager.SetAppSettingsProvider(Configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();

            services.AddAntiforgery();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                //Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(7200);

                options.Cookie.HttpOnly = true;
                // Make the session cookie essential
                options.Cookie.IsEssential = true;
            });

            //DI
            services.AddTransient(r => new VersionClient(Configuration["buildNumber"], Configuration["InsightsInstrumentationKey"]));

            //Add service and create cors policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    //.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .Build()
                );
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Compression
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<FormOptions>(x => x.ValueCountLimit = 36700160);

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services
                .AddMvc(config =>
                {
                    config.EnableEndpointRouting = false;
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                    if (options.SerializerSettings != null)
                    {
                        options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                    }
                });

            services.AddHttpContextAccessor();
            var instrumentationKey = Configuration["InsightsInstrumentationKey"];
            services.AddApplicationInsightsTelemetry(instrumentationKey);
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:CotorriaAzureBlobConnectionString:blob"], preferMsi: true);
            });
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:CotorriaAzureBlobConnectionString:blob"], preferMsi: true);
            });

            services.AddSingleton<MemoryCache>();
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (ex is EntryPointNotFoundException) code = HttpStatusCode.NotFound;
            else if (ex is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;
            else if (ex is InvalidCastException) code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new { ExceptionMessage = ex.ToString(), Message = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCaching();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            //Set CORS behaviors
            app.Use(async (context, next) =>
            {

                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "gid,iid,ssid,srid,inid,cid,rfc,lsid,aid,lid,Content-Type,User-Agent,Origin,Referer");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST,GET,DELETE,PUT,HEAD");
                    context.Response.Headers.Add("Access-Control-Request-Headers", "*");
                    context.Response.Headers.Add("Access-Control-Request-Method", "*");

                    return Task.FromResult(0);
                });

                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(context, ex);
                }
            });

            //Set index.html as default page
            DefaultFilesOptions DefaultFile = new DefaultFilesOptions();
            DefaultFile.DefaultFileNames.Clear();
            DefaultFile.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(DefaultFile);

            //Use static files, for html files
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });

            System.Web.HttpContext.Configure(app.ApplicationServices.
               GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
            );

            app.UseResponseCompression();

            app.UseSession();

            app.UseDefaultFiles();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseCookiePolicy();

            var defaultDateCulture = "es-ES";
            var ci = new System.Globalization.CultureInfo(defaultDateCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<System.Globalization.CultureInfo>
                {
                    ci,
                },
                SupportedUICultures = new List<System.Globalization.CultureInfo>
                {
                    ci,
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }
    }

}