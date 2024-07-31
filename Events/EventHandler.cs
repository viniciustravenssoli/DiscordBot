using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Events
{
    public class EventHandlers
    {
        public static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            sender.Logger.LogInformation("Bot is ready!");
            return Task.CompletedTask;
        }

        public static async Task OnGuildMemberAdded(DiscordClient sender, GuildMemberAddEventArgs e)
        {
            var welcomeChannel = e.Guild.GetDefaultChannel();
            var welcomeEmbed = new DiscordEmbedBuilder
            {
                Title = "Bem-vindo!",
                Description = $"{e.Member.Mention}, bem-vindo ao servidor {e.Guild.Name}! Estamos felizes em tê-lo(a) aqui.",
                Color = DiscordColor.Blurple
            };

            await welcomeChannel.SendMessageAsync(embed: welcomeEmbed);
        }

        public static async Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().Contains("hello"))
            {
                await e.Message.RespondAsync("Hi there!");
            }
        }

        public static async Task OnReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            if (e.Emoji.Name == "👍")
            {
                var user = e.User;
                var message = e.Message;
                await message.RespondAsync($"{user.Mention} liked this message!");
            }
        }
    }
}