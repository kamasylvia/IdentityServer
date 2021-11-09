﻿using System;
using System.Reflection;
using FileService.Data;
using FileService.Data.Repositories;
using FileService.Extensions;
using FileService.Services.FileStorageService;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileService
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
            services.AddControllers();
            services.AddGrpc();

            #region Swagger
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileService", Version = "v1" });
                }
            );
            #endregion

            #region Database
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(
                options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            );
            #endregion

            #region DI
            #region Services
            //services.AddScoped<IFileStorageService, FileStorageService>();
            #endregion

            #region Repositories
            //services.AddScoped<IAppFileRepository, AppFileRepository>();
            #endregion
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddCustomDependencies();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Seed data
            DbInitializer.CreateStorageDirectory(app: app);
            DbInitializer.Initialize(app: app, environment: env);
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(
                    c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileService v1")
                );
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapGrpcService<FileStorageService>();
                    endpoints.MapControllers();
                }
            );
        }
    }
}
