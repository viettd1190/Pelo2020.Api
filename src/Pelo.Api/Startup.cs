using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pelo.Api.Commons;
using Pelo.Api.Services.UserServices;
using Pelo.Common.Extensions;
using Pelo.Common.Kafka;
using Pelo.Common.Log.AutoWriteLog;
using Pelo.Common.Repositories;
using Swashbuckle.AspNetCore.Swagger;

namespace Pelo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
                                                    {
                                                        options.CheckConsentNeeded = context => true;
                                                        options.MinimumSameSitePolicy = SameSiteMode.None;
                                                    });

            services.Configure<ApiBehaviorOptions>(options =>
                                                   {
                                                       options.SuppressModelStateInvalidFilter = true;
                                                   });

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var producerConfig = new ProducerConfig();
            var consumerConfig = new ConsumerConfig();
            Configuration.Bind("producer", producerConfig);
            Configuration.Bind("consumer", consumerConfig);
            services.AddSingleton(producerConfig);
            services.AddSingleton(consumerConfig);

            #region Swagger

            services.AddSwaggerGen(options =>
                                   {
                                       options.SwaggerDoc("v1.0",
                                                          new Info
                                                          {
                                                                  Title = AppUtil.SWAGGER_TITLE_V1,
                                                                  Version = AppUtil.SWAGGER_VERSION_V1
                                                          });

                                       options.AddSecurityDefinition("Bearer",
                                                                     new ApiKeyScheme
                                                                     {
                                                                             In = "header",
                                                                             Description = "Please enter into field the word 'Bearer' following by space and JWT",
                                                                             Name = "Authorization",
                                                                             Type = "apiKey"
                                                                     });
                                       options.AddSecurityDefinition("Permission",
                                                                     new ApiKeyScheme
                                                                     {
                                                                             In = "header",
                                                                             Description = "Please enter into field permission id",
                                                                             Name = "PermissionId",
                                                                             Type = "apiKey"
                                                                     });
                                       options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                                                                      {
                                                                              {"Bearer", Enumerable.Empty<string>()},
                                                                              {"Permission", Enumerable.Empty<string>()}
                                                                      });

                                       var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                                       var xmlPath = Path.Combine("App_Data",
                                                                  xmlFile);
                                       options.IncludeXmlComments(xmlPath);

                                       options.DocInclusionPredicate((docName,
                                                                      apiDesc) =>
                                                                     {
                                                                         var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                                                                         if(actionApiVersionModel == null) return true;

                                                                         if(actionApiVersionModel.DeclaredApiVersions.Any())
                                                                             return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);

                                                                         return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                                                                     });
                                       options.OperationFilter<ApiVersionOperationFilter>();
                                   });
            services.AddApiVersioning(o =>
                                      {
                                          o.DefaultApiVersion = new ApiVersion(1,
                                                                               0);
                                          o.AssumeDefaultVersionWhenUnspecified = true;
                                          o.ApiVersionReader = new MediaTypeApiVersionReader();
                                          o.ReportApiVersions = true;
                                      });

            #endregion

            #region Register autofac

            var builder = new ContainerBuilder();

            var connectionString = Configuration.GetConnectionString("Db");

            builder.RegisterAssemblyTypes(typeof(DapperReadOnlyRepository).Assembly)
                   .Where(c => c.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces()
                   .WithParameter("connectionString",
                                  connectionString)
                   .EnableInterfaceInterceptors()
                   .InterceptedBy("log-calls")
                   .InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(AccountService).Assembly)
                   .Where(c => c.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .WithParameter("secretKey",
                                  Configuration["Authentication:SecretKey"])
                   .EnableInterfaceInterceptors()
                   .InterceptedBy("log-calls")
                   .InstancePerLifetimeScope();
            builder.RegisterType<HttpContextAccessor>()
                   .As<IHttpContextAccessor>()
                   .SingleInstance();

            builder.RegisterType<BusPublisher>()
                   .As<IBusPublisher>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<BusSubscriber>()
                   .As<IBusSubscriber>()
                   .InstancePerLifetimeScope();
            builder.Register(c => new DynamicProxyLog(new DynamicProxyAsyncLog(Configuration)))
                   .Named<IInterceptor>("log-calls");

            builder.AddDispatchers();

            builder.Populate(services);
            var container = builder.Build();

            var serviceProvider = new AutofacServiceProvider(container);

            #endregion

            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env)
        {
            if(env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseDefaultFiles();

            app.UseCors(x => x
                             .AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader());

            app.UseMiddleware<ApiLoggingMiddleware>();

            app.UseMvc(routes =>
                       {
                           routes.MapRoute("default",
                                           "{controller=Home}/{action=Index}/{id?}");
                       });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
                             {
                                 options.SwaggerEndpoint("/swagger/v1.0/swagger.json",
                                                         AppUtil.SWAGGER_TITLE_V1);
                                 options.RoutePrefix = string.Empty;
                             });
        }
    }
}
