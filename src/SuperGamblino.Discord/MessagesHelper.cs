using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.Messages;

namespace SuperGamblino.Discord
{
    internal static class MessagesHelper
    {
        internal static DiscordEmbed ToDiscordEmbed(this Message message)
        {
            return new DiscordEmbedBuilder()
                .WithColor(new DiscordColor(message.Color))
                .WithTitle(message.Title)
                .WithAuthor(message.Author)
                .WithDescription(message.Description)
                .WithFooter(message.Footer)
                .WithFields(message.Fields)
                .Build();
        }

        internal static async Task<DiscordEmbed> ToDiscordEmbed(this Task<Message> message)
        {
            return ToDiscordEmbed(await message);
        }

        //Do this require 'builder = builder.AddField' or 'builder.AddField' is enough?
        private static DiscordEmbedBuilder WithFields(this DiscordEmbedBuilder builder, IEnumerable<Field> fields)
        {
            return fields.Aggregate(builder, (current, field) => current.AddField(field.Name, field.Value));
        }
    }
}