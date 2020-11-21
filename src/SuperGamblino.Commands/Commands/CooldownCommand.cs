using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperGamblino.Core.CommandsObjects;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class CooldownCommand
    {
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public CooldownCommand(UsersConnector usersConnector, MessagesHelper messagesHelper)
        {
            _usersConnector = usersConnector;
            _messagesHelper = messagesHelper;
        }

        //TODO Clean it up!!!
        //TODO Test it up!!!
        public async Task<Message> GetCooldowns(ulong userId)
        {
            var user = await _usersConnector.GetUser(userId);
            var curLhr = user.LastHourlyReward;
            var curLdr = user.LastDailyReward;
            var curLwr = user.LastWorkReward;
            ;
            var curLvr = user.LastVoteReward;
            ;
            var cooldownObjects = new List<CooldownObject>();

            if (curLhr.HasValue)
            {
                var timeSpan = DateTime.Now - curLhr.Value;
                cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(1)
                    ? new CooldownObject("Hourly")
                    : new CooldownObject("Hourly", TimeSpan.FromHours(1) - timeSpan));
            }
            else
            {
                cooldownObjects.Add(new CooldownObject("Hourly"));
            }

            if (curLdr.HasValue)
            {
                var timeSpan = DateTime.Now - curLdr.Value;
                cooldownObjects.Add(timeSpan >= TimeSpan.FromDays(1)
                    ? new CooldownObject("Daily")
                    : new CooldownObject("Daily", TimeSpan.FromDays(1) - timeSpan));
            }
            else
            {
                cooldownObjects.Add(new CooldownObject("Daily"));
            }

            if (curLwr.HasValue)
            {
                var timeSpan = DateTime.Now - curLwr.Value;
                cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(6)
                    ? new CooldownObject("Work")
                    : new CooldownObject("Work", TimeSpan.FromHours(6) - timeSpan));
            }
            else
            {
                cooldownObjects.Add(new CooldownObject("Work"));
            }

            if (curLvr.HasValue)
            {
                var timeSpan = DateTime.Now - curLvr.Value;
                cooldownObjects.Add(timeSpan >= TimeSpan.FromHours(12)
                    ? new CooldownObject("Vote")
                    : new CooldownObject("Vote", TimeSpan.FromHours(12) - timeSpan));
            }
            else
            {
                cooldownObjects.Add(new CooldownObject("Vote"));
            }

            return _messagesHelper.ListCurrentCooldowns(cooldownObjects, "Cooldowns");
        }
    }
}