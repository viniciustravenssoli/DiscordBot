using System;
using System.IO;
using System.Threading.Tasks;
using DiscordBot.Commands;
using DiscordBot.Events;
using DiscordBot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        static async Task Main(string[] args)
        {

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            var token = configuration["Discord:Token"];


            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
            };

            Client = new DiscordClient(discordConfig);
            Client.GuildMemberAdded += EventHandlers.OnGuildMemberAdded;
            Client.MessageCreated += EventHandlers.OnMessageCreated;
            Client.MessageReactionAdded += EventHandlers.OnReactionAdded;

            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(5)
            });

            Client.Ready += EventHandlers.Client_Ready;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "!", "%" },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
                Services = new ServiceCollection()
                    .AddSingleton(new GuessingGame())
                    .BuildServiceProvider(),
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<TestCommands>();

            await Client.ConnectAsync(new DiscordActivity("DiscordBOT", ActivityType.Playing), UserStatus.Online);
            await Task.Delay(-1);
        }
    }
}
