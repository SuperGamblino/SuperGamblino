using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.CommandsLogics;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class CollectDrop
    {
        private CollectDropCommandLogic _logic;
        private readonly CoindropConnector _coindropConnector;
        private readonly Config _config;
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public CollectDrop(Config config, CoindropConnector coindropConnector, UsersConnector usersConnector, Messages messages, CollectDropCommandLogic logic)
        {
            _config = config;
            _coindropConnector = coindropConnector;
            _usersConnector = usersConnector;
            _messages = messages;
            _logic = logic;
        }

        [Command("collect")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Collects the reward from a coindrop.")]
        public async Task OnExecute(CommandContext command)
        {
            var arguments = command.RawArgumentString;
            await command.RespondAsync("", false,
                await _logic.Collect(arguments, command.Channel.Id, command.User.Id, command.User.Username));
        }
    }
}