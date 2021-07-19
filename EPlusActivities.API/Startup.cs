using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EPlusActivities.API.Data;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure;
using EPlusActivities.API.Infrastructure.Identity;
using EPlusActivities.API.Infrastructure.Repositories;
using EPlusActivities.API.Services;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace EPlusActivities.API
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(
            IWebHostEnvironment environment,
            IConfiguration configuration
        )
        {
            Environment = environment;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString =
                Configuration.GetConnectionString("DefaultConnection");

            // var serverVersion = ServerVersion.AutoDetect(connectionString);
            var migrationsAssembly =
                typeof (Startup).GetTypeInfo().Assembly.GetName().Name;

            services
                .AddControllers()
                .AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler =
                        ReferenceHandler.Preserve);

            services.AddHttpClient();

            services
                .AddSwaggerGen(c =>
                {
                    c
                        .SwaggerDoc("v1",
                        new OpenApiInfo {
                            Title = "IdentityServer",
                            Version = "v1"
                        });
                });

            // 数据库配置系统应用用户数据上下文
            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options
                        .UseMySql(connectionString,
                        o =>
                            o
                                .ServerVersion(new Version(8, 0, 25),
                                ServerType.MySql)));

            // 启用 Identity 服务 添加指定的用户和角色类型的默认标识系统配置
            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 启用数据库仓库
            services
                .AddTransient<IActivityRepository, ActivityRepository>()
                .AddTransient<IAttendanceRepository, AttendanceRepository>()
                .AddTransient
                <IFindByUserIdRepository<Address>, AddressRepository>()
                .AddTransient
                <IFindByUserIdRepository<Lottery>, LotteryRepository>()
                .AddTransient
                <IFindByNameRepository<PrizeItem>, PrizeItemRepository>()
                .AddTransient<INameExistsRepository<Brand>, BrandRepository>()
                .AddTransient
                <INameExistsRepository<Category>, CategoryRepository>()
                .AddTransient
                <IFindByUserIdRepository<PrizeType>, PrizeTypeRepository>();

            // 启用短信服务
            services.AddTransient<ISmsService, SmsService>();

            var builder =
                services
                    .AddIdentityServer(options =>
                    {
                        options.Events.RaiseErrorEvents = true;
                        options.Events.RaiseInformationEvents = true;
                        options.Events.RaiseFailureEvents = true;
                        options.Events.RaiseSuccessEvents = true;

                        // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                        options.EmitStaticAudienceClaim = true;
                    }) // .AddTestUsers(TestUsers.Users)
                    .AddAspNetIdentity<ApplicationUser>() // .AddProfileService<ProfileService>() // SMS Validator
                    .AddExtensionGrantValidator<SmsGrantValidator>() // this adds the config data from memory (clients, resources, CORS)
                    .AddInMemoryIdentityResources(Config.IdentityResources)
                    .AddInMemoryApiScopes(Config.ApiScopes)
                    .AddInMemoryApiResources(Config.ApiResources)
                    .AddInMemoryClients(Config.Clients);

            // this adds the config data from DB (clients, resources, CORS)
            /*
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseMySql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                    builder.UseMySql(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
            });
            */
            // not recommended for production - you need to store your key material somewhere secure
            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }

            // 受保护的 API 设置
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //IdentityServer地址
                    options.Authority = "http://localhost:52537";

                    //对应Idp中ApiResource的Name
                    options.Audience = "eplus.api";

                    //不使用https
                    options.RequireHttpsMetadata = false;
                });

            //基于策略授权
            services
                .AddAuthorization(options =>
                {
                    options
                        .AddPolicy("EPlusPolicy",
                        builder =>
                        {
                            builder.RequireRole("admin", "manager", "customer");
                            builder.RequireScope("eplus.scope");
                        });
                    options
                        .AddPolicy("TestPolicy",
                        builder =>
                        {
                            builder.RequireRole("admin", "manager", "customer");
                            builder.RequireClaim("phone_number");
                        });
                });

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
        )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app
                    .UseSwaggerUI(c =>
                        c
                            .SwaggerEndpoint("/swagger/v1/swagger.json",
                            "IdentityServer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthentication();

            app.UseAuthorization();

            DbInitializer.Initialize (env, context, userManager, roleManager);

            app
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
