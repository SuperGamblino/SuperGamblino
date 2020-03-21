using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino
{
    class Messages
    {
        public static async Task Won (CommandContext command, int bet)
        {
            int currentCred = Database.CommandGiveCredits(command.User.Id, bet * 2);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorSuccess),
                Description = "You've won!\n\nCurrent credits: " + currentCred
            };
            await command.RespondAsync("", false, message);
        }
        public static async Task Lost (CommandContext command)
        {
            int currentCred = Database.CommandGetUserCredits(command.User.Id);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorWarning),
                Description = "You've lost...\n\nCurrent credits: " + currentCred

            };
            await command.RespondAsync("", false, message);
        }
        public static async Task NotEnoughCredits (CommandContext command)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorWarning),
                Description = "This is a casino, not a bank!\n\nYou do not have enough credits."

            };
            await command.RespondAsync("", false, message);
        }
        public static async Task InvalidArgument (CommandContext command, string[] arguments)
        {
            
            string desc = "Invalid arugment.\nUse the following " +  command.Command.ToString() + " ";
            foreach (string arg in arguments)
            {
                desc += arg + ",";
            }
            desc = desc.Remove(desc.Length - 1, 1);
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorWarning),
                Description = desc
            };
            await command.RespondAsync("", false, message);
        }

        public static async Task CoinsGain(CommandContext command, int ammount)
        {
            DiscordEmbed message = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(Config.colorSuccess),
                Description = "You've gained: " + ammount + " coins!"
            };
            await command.RespondAsync("",false, message);
        }
    }
}
