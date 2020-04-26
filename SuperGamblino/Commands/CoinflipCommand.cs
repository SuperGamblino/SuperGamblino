using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;
using SuperGamblino.Helpers;

namespace SuperGamblino.Commands
{
    internal class CoinflipCommand
    {
        private readonly CoinflipCommandLogic _logic;

        public CoinflipCommand(CoinflipCommandLogic logic)
        {
            _logic = logic;
        }

        [Command("coinflip")]
        [Aliases("cf")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Simple coinflip game. This command takes two argument Head or Tail and your bet. \nEx. coinflip head 100")]
        public async Task OnExecute(CommandContext command)
        {
            var arguments = command.RawArgumentString;
            var userId = command.User.Id;
            await command.RespondAsync("",false,await _logic.PlayCoinflip(arguments, userId));
        }
    }
}