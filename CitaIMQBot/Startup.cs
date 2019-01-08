using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitaIMQBot.Bots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CitaIMQBot
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup( IHostingEnvironment env )
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddSingleton(configuration);

            services.AddSingleton<CitaIMQBotAccessors>(sp =>
            {
                // We need to grab the conversationState we added on the options in the previous step

                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                var userState = options.State.OfType<UserState>().FirstOrDefault();

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new CitaIMQBotAccessors(conversationState, userState)
                {
                    DialogStateAccessor = conversationState.CreateProperty<DialogState>("DialogState"),
                    CitaIMQState = userState.CreateProperty<CitaIMQBotState>("CitaIMQState"),
                };

                return accessors;
            });
            services.AddBot<Bots.CitaIMQBot>(options =>
            {
                var conversationState = new ConversationState(new MemoryStorage());
                var userState = new UserState(new MemoryStorage());
                options.State.Add(conversationState);
                options.State.Add(userState);
                options.CredentialProvider = new ConfigurationCredentialProvider(configuration);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseFileServer();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
