using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;

namespace SuperGamblino.Commands
{
    internal class CollectDrop
    {
        private readonly CoindropConnector _coindropConnector;
        private readonly Config _config;
        private readonly Messages _messages;
        private readonly UsersConnector _usersConnector;

        public CollectDrop(Config config, CoindropConnector coindropConnector, UsersConnector usersConnector, Messages messages)
        {
            _config = config;
            _coindropConnector = coindropConnector;
            _usersConnector = usersConnector;
            _messages = messages;
        }

        [Command("collect")]
        [Cooldown(1, 2, CooldownBucketType.User)]
        [Description("Collects the reward from a coindrop.")]
        public async Task OnExecute(CommandContext command)
        {



            try
            {
                var argument = command.RawArgumentString.ToUpper().TrimStart().Split(' ');

                if (argument.Length == 1)
                {
                    int claimId = Convert.ToInt32(argument[0].Trim());
                    int reward = await _coindropConnector.CollectCoinDrop(command.Channel.Id, claimId, command);
                    if (reward != 0)
                    {
                        await _messages.CoindDropSuccess(command, reward);
                        await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                    }
                    else
                        await _messages.CoindDropLate(command);
                }
                else
                {
                    await _messages.InvalidArgument(command, new[] { "<Claim Id>" });
                }
            }
            catch(Exception ex)
            {
                await _messages.InvalidArgument(command, new[] { "<Claim Id>" });
            }
        }
    }
}