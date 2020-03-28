using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SuperGamblino.GameObjects;
using static SuperGamblino.GameObjects.Work;

namespace SuperGamblino.Commands
{
    class WorkReward
    {
        private readonly Database _database;
        private readonly Messages _messages;

        public WorkReward(Messages messages, Database database)
        {
            _messages = messages;
            _database = database;
        }

        [Command("work")]
        [Aliases("get-work")]
        [Description("Gives you credits based on your job. This command has no arguments.")]
        public async Task OnExecute(CommandContext command)
        {
            User user = await _database.GetUser(command);
            DateTimeResult lastReward = await _database.GetDateTime(command.User.Id, "last_work_reward");

            Job currentJob = GetCurrentJob(user.Level);

            

            if (lastReward.Successful)
            {
                if (lastReward.DateTime.HasValue)
                {
                    TimeSpan timeSpan = DateTime.Now - lastReward.DateTime.Value;
                    if (timeSpan >= TimeSpan.FromHours(currentJob.Cooldown))
                    {
                        await _database.CommandGiveCredits(command.User.Id, currentJob.Reward);
                        await _database.SetDateTime(command.User.Id, "last_work_reward", DateTime.Now);
                        await _messages.CoinsGain(command, currentJob.Reward);
                    }
                    else
                    {
                        await _messages.TooEarly(command, TimeSpan.FromHours(currentJob.Cooldown) - timeSpan);
                    }
                }
                else
                {
                    await _database.CommandGiveCredits(command.User.Id, currentJob.Reward);
                    await _database.SetDateTime(command.User.Id, "last_work_reward", DateTime.Now);
                    await _messages.CoinsGain(command, currentJob.Reward);
                }
            }
            else
            {
                await _messages.Error(command, "Some problem with DB occured!");
            }

        }
    }
}