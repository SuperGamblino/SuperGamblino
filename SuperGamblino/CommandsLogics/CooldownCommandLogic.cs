using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.CommandsLogics
{
    public class CooldownCommandLogic
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;

        public CooldownCommandLogic(UsersConnector usersConnector, Messages messages)
        {
            _usersConnector = usersConnector;
            _messages = messages;
        }

        public async Task<DiscordEmbed> GetCooldowns(ulong userId)
        {
            var curLhr = await _usersConnector.GetDateTime(userId, "last_hourly_reward");
            var curLdr = await _usersConnector.GetDateTime(userId, "last_daily_reward");
            var curLwr = await _usersConnector.GetDateTime(userId, "last_work_reward");
            var curLvr = await _usersConnector.GetDateTime(userId, "last_vote_reward");
            var cooldownObjects = new List<CooldownObject>();

            if (curLhr.Successful && curLdr.Successful) //TODO Should I add curLwr and curLvr or should I correct the unit test?
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
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromDays(1)
                        ? new CooldownObject("Daily")
                        : new CooldownObject("Daily", TimeSpan.FromDays(1) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Daily"));
                }

                if (curLwr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLwr.DateTime.Value;
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(6)
                        ? new CooldownObject("Work")
                        : new CooldownObject("Work", TimeSpan.FromHours(6) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Work"));
                }
                if (curLvr.DateTime.HasValue)
                {
                    var timeSpan = DateTime.Now - curLvr.DateTime.Value;
                    cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(12)
                        ? new CooldownObject("Vote")
                        : new CooldownObject("Vote", TimeSpan.FromHours(12) - timeSpan));
                }
                else
                {
                    cooldownObjects.Add(new CooldownObject("Vote"));
                }

                return _messages.ListCurrentCooldowns(cooldownObjects, "Cooldowns");
            }
            else
            {
                return _messages.Error("Some problem with DB occured!!!");
            }
        }
    }
}