// ----------------------------------------------------------------------------
//       ___ ___  ___     ___
// |  | |__   |  |__  \_/  |
// |/\| |___  |  |___ / \  |
//
// Yet another WeText application for demonstration.
// MIT License
//
// Copyright (c) 2018 Sunny Chen (daxnet)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ----------------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using WeText.Common;
using WeText.CrudProviders.MongoDB;

namespace WeText.Services.Accounts
{
    public class Startup
    {
        #region Private Fields

        private readonly ILogger logger;

        #endregion Private Fields

        #region Public Constructors

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion Public Properties

        #region Public Methods

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeText Accounts Service");
            });

            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "WeText Accounts Service",
                    Version = "v1",
                    Description = "The RESTful API for WeText user accounts."
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xml = Path.Combine(basePath, "WeText.Services.Accounts.xml");
                if (File.Exists(xml))
                {
                    options.IncludeXmlComments(xml);
                }
            });

            RegisterApplicationServices(services);
        }

        #endregion Public Methods

        #region Private Methods

        private void RegisterApplicationServices(IServiceCollection services)
        {
            var mongoServerHost = Configuration["mongo:server:host"];
            if (string.IsNullOrEmpty(mongoServerHost))
            {
                mongoServerHost = "localhost";
            }

            if (!int.TryParse(Configuration["mongo:server:port"], out var mongoServerPort))
            {
                mongoServerPort = 27017;
            }

            var mongoDatabase = Configuration["mongo:server:database"];
            if (string.IsNullOrEmpty(mongoDatabase))
            {
                mongoDatabase = "WeText_Accounts";
            }

            services.AddTransient<ICrudProvider>(serviceProvider => new MongoCrudProvider(mongoDatabase, mongoServerHost, mongoServerPort));
        }

        #endregion Private Methods
    }
}