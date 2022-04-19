﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrchestratorBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            OrchestratorConfig = configuration.GetSection("Orchestrator").Get<OrchestratorConfig>();
        }

        public OrchestratorConfig OrchestratorConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton(InitializeOrchestrator());

            // Create the bot services (LUIS, QnA) as a singleton.
            services.AddSingleton<IBotServices, BotServices>();

            // Create the bot as a transient.
            services.AddTransient<IBot, DispatchBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }

        private OrchestratorRecognizer InitializeOrchestrator()
        {
            var modelFolder = Path.GetFullPath(OrchestratorConfig.ModelFolder);
            var snapshotFile = Path.GetFullPath(OrchestratorConfig.SnapshotFile);
            var orc = new OrchestratorRecognizer()
            {
                ModelFolder = modelFolder,
                SnapshotFile = snapshotFile
            };
            return orc;
        }
    }
}
