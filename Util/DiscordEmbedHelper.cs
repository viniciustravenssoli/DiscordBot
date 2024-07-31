using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace DiscordBot.Util
{
    public class DiscordEmbedHelper
    {
        public DiscordEmbedBuilder CreateEmbed(string title, string description, DiscordColor color)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = color
            };

            return embed;
        }

        public DiscordEmbedBuilder CreateEmbedWarningMessage(string title, string description)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = new DiscordColor(0xFFFF00)
            };

            return embed;
        }

        public DiscordEmbedBuilder CreateEmbedErrorMessage(string title, string description)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = new DiscordColor(0xFF0000)
            };

            return embed;
        }

        public DiscordEmbedBuilder CreateEmbedSuccessMessage(string title, string description)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = title,
                Description = description,
                Color = new DiscordColor(0x00ff00)
            };

            return embed;
        }

        public DiscordEmbedBuilder CreateEmbedMoreOptions(string userName, string userAvatarUrl)
        {
            var embed = new DiscordEmbedBuilder
            {   
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    Name = userName,
                    IconUrl = userAvatarUrl
                },
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = userAvatarUrl
                },
                Color = new DiscordColor(0x4286f4),
            };

            return embed;
        }

    }
}