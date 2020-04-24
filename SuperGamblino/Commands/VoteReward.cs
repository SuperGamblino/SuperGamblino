using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.Commands
{
    internal class VoteReward
    {
        private readonly UsersConnector _usersConnector;
        private readonly Messages _messages;
        private readonly Config _config;

        public VoteReward(Messages messages, UsersConnector usersConnector, Config config)
        {
            _messages = messages;
            _usersConnector = usersConnector;
            _config = config;
        }

        [Command("vote")]
        [Description("This command gives you 400 credits. It's usable every 12th hour.")]
        public async Task OnExecute(CommandContext command)
        {
            throw new NotImplementedException();
            const int reward = 400;
            HttpClient client = new HttpClient();
            var result = await _usersConnector.GetDateTime(command.User.Id, "last_vote_reward");
            if (result.Successful)
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://top.gg/api/bots/688160933574475800/check?userId={command.User.Id}"),
                    Headers = {
                        { HttpRequestHeader.Authorization.ToString(), _config.BotSettings.TopGgToken }
                    }
                };

                var response = await client.SendAsync(httpRequestMessage);
                var didUserVote = JsonConvert.DeserializeObject<Vote>(await response.Content.ReadAsStringAsync());

                if (didUserVote.voted)
                {
                    if (result.DateTime.HasValue)
                    {
                        var timeSpan = DateTime.Now - result.DateTime.Value;
                        if (timeSpan >= TimeSpan.FromHours(12))
                        {
                            await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                            await _usersConnector.SetDateTime(command.User.Id, "last_vote_reward", DateTime.Now);
                            await _messages.CoinsGain(command, reward);
                        }
                        else
                        {
                            await _messages.TooEarly(command, TimeSpan.FromHours(12) - timeSpan);
                        }
                    }
                    else
                    {
                        await _usersConnector.CommandGiveCredits(command.User.Id, reward);
                        await _usersConnector.SetDateTime(command.User.Id, "last_vote_reward", DateTime.Now);
                        await _messages.CoinsGain(command, reward);
                    }
                }
                else
                {
                    await _messages.NotVotedYet(command);
                }
                
            }
            else
            {
                await _messages.Error(command, "Some problem with DB occured!");
            }
        }
    }
}