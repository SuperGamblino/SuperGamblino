using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.Commands
{
    internal class Cooldown
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;

        public Cooldown(Messages messages, UsersConnector usersConnector)
        {
            _messages = messages;
            _usersConnector = usersConnector;
        }

        [Command("cooldown")]
        [Aliases("cd")]
        [Cooldown(1, 3, CooldownBucketType.User)]
        [Description("Shows the current command cooldowns. This command takes no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            throw new NotImplementedException();
            var curLhr = await _usersConnector.GetDateTime(command.User.Id, "last_hourly_reward");
            var curLdr = await _usersConnector.GetDateTime(command.User.Id, "last_daily_reward");
            var curLwr = await _usersConnector.GetDateTime(command.User.Id, "last_work_reward");
            var curLvr = await _usersConnector.GetDateTime(command.User.Id, "last_vote_reward");
            var cooldownObjects = new List<CooldownObject>();

            if (curLhr.Successful && curLdr.Successful)
            {
                if (curLhr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLhr.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(1))
                        cooldownObjects.Add(new CooldownObject("Hourly"));
                    else
                        cooldownObjects.Add(new CooldownObject("Hourly", TimeSpan.FromHours(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Hourly"));
                }

                if (curLdr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLdr.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromDays(1))
                        cooldownObjects.Add(new CooldownObject("Daily"));
                    else
                        cooldownObjects.Add(new CooldownObject("Daily", TimeSpan.FromDays(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Daily"));
                }

                if (curLwr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLwr.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(6))
                        cooldownObjects.Add(new CooldownObject("Work"));
                    else
                        cooldownObjects.Add(new CooldownObject("Work", TimeSpan.FromHours(6) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Work"));
                }
                if (curLvr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLvr.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(12))
                        cooldownObjects.Add(new CooldownObject("Vote"));
                    else
                        cooldownObjects.Add(new CooldownObject("Vote", TimeSpan.FromHours(12) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Vote"));
                }

                await _messages.ListCooldown(command, cooldownObjects);
            }
            else
            {
                await _messages.Error(command, "Some problem with DB occured!");
            }
        }
    }
}