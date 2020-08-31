using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using HotChocolate.AspNetCore;
using HotChocolate;
using System.Reflection;
using System;
using AutoMapper;
using Cotorra.Schema.DTO;
using Cotorra.Schema.DTO.Catalogs;

namespace Cotorra.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Cotorria WebAPI",
                    Description = "Web API del Proyecto de Cotorria, encaminado a publicar la funcionalidad de nóminas.",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" },
                             Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },  new string[] { } }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .Build()
                );
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 209715200;
                options.ValueCountLimit = 209715200;
                options.ValueLengthLimit = 209715200;
            });

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "image/svg+xml" });
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            services.AddTransient<IValidator<JobPosition>, JobPositionValidator>();
            services.AddTransient<IBaseRecordManager<JobPosition>, BaseRecordManager<JobPosition>>();
            services.AddTransient(r => new CotorraNode.Layer2.Company.Client.InstanceClient(Configuration["companyhost"]));

            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.ReturnHttpNotAcceptable = true;
            })
                .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();

                if (options.SerializerSettings != null)
                {
                    options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                }
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //ApplicationInsights
            var instrumentationKey = Configuration["InsightsInstrumentationKey"];
            services.AddApplicationInsightsTelemetry(instrumentationKey);

            services.AddGraphQL(SchemaBuilder.New()
                .AddQueryType<Query>()
                .ModifyOptions(o => o.RemoveUnreachableTypes = true)
                .Create());
            services.AddDataLoaderRegistry();
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<ICatalogServiceSearch<Department, DepartmentDTO>, CatalogServiceSearch<Department, DepartmentDTO>>();
            services.AddTransient<ICatalogServiceSearch<Area, AreaDTO>, CatalogServiceSearch<Area, AreaDTO>>();
            services.AddTransient<ICatalogServiceSearch<MinimunSalary, MinimumSalaryDTO>, CatalogServiceSearch<MinimunSalary, MinimumSalaryDTO>>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IAreasService, AreasService>();
            services.AddTransient<ICatalogServiceSearch<MonthlyIncomeTax, MonthlyIncomeTaxDTO>, CatalogServiceSearch<MonthlyIncomeTax, MonthlyIncomeTaxDTO>>();
            services.AddTransient<ICatalogServiceSearch<MonthlyEmploymentSubsidy, MonthlyEmploymentSubsidyDTO>, CatalogServiceSearch<MonthlyEmploymentSubsidy, MonthlyEmploymentSubsidyDTO>>();
            services.AddTransient<ICatalogServiceSearch<AnualIncomeTax, AnualIncomeTaxDTO>, CatalogServiceSearch<AnualIncomeTax, AnualIncomeTaxDTO>>();
            services.AddTransient<ICatalogServiceSearch<AnualEmploymentSubsidy, AnualEmploymentSubsidyDTO>, CatalogServiceSearch<AnualEmploymentSubsidy, AnualEmploymentSubsidyDTO>>();
            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<ICatalogServiceSearch<InfonavitInsurance, InfonavitInsuranceDTO>, CatalogServiceSearch<InfonavitInsurance, InfonavitInsuranceDTO>>();
            services.AddTransient<ICatalogServiceSearch<IMSSEmployerTable, IMSSEmployerTableDTO>, CatalogServiceSearch<IMSSEmployerTable, IMSSEmployerTableDTO>>();
            services.AddTransient<ICatalogServiceSearch<IMSSEmployeeTable, IMSSEmployeeTableDTO>, CatalogServiceSearch<IMSSEmployeeTable, IMSSEmployeeTableDTO>>();
            services.AddTransient<ICatalogServiceSearch<SGDFLimits, SGDFLimitsDTO>, CatalogServiceSearch<SGDFLimits, SGDFLimitsDTO>>();
            services.AddTransient<ICatalogServiceSearch<Employee, EmployeeDTO>, CatalogServiceSearch<Employee, EmployeeDTO>>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cotorria API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

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


                await next();
            });

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app
           .UseGraphQL("/graphql")
           .UsePlayground("/graphql");
        }
    }
}
