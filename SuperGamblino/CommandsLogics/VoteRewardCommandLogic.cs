using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using SuperGamblino.DatabaseConnectors;
using SuperGamblino.GameObjects;

namespace SuperGamblino.CommandsLogics
{
    public class VoteRewardCommandLogic
    {
        private readonly HttpClient _client;
        private readonly Config _config;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public VoteRewardCommandLogic(HttpClient client, UsersConnector usersConnector, Config config,
            MessagesHelper messagesHelper)
        {
            _client = client;
            _usersConnector = usersConnector;
            _config = config;
            _messagesHelper = messagesHelper;
        }

        public async Task<DiscordEmbed> Vote(ulong userId)
        {
            const int reward = 400;
            var result = await _usersConnector.GetDateTime(userId, "last_vote_reward");
            if (result.Successful)
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://top.gg/api/bots/688160933574475800/check?userId={userId}"),
                    Headers =
                    {
                        {HttpRequestHeader.Authorization.ToString(), _config.BotSettings.TopGgToken}
                    }
                };

                var response = await _client.SendAsync(httpRequestMessage);
                var didUserVote = JsonConvert.DeserializeObject<Vote>(await response.Content.ReadAsStringAsync());

                if (didUserVote.voted)
                {
                    if (result.DateTime.HasValue)
                    {
                        var timeSpan = DateTime.Now - result.DateTime.Value;
                        if (timeSpan >= TimeSpan.FromHours(12))
                        {
                            await _usersConnector.CommandGiveCredits(userId, reward);
                            await _usersConnector.SetDateTime(userId, "last_vote_reward", DateTime.Now);
                            return _messagesHelper.CreditsGain(reward, "TopGGVote");
                        }

                        return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromHours(12) - timeSpan, "!vote",
                            "TopGGVote");
                    }

                    await _usersConnector.CommandGiveCredits(userId, reward);
                    await _usersConnector.SetDateTime(userId, "last_vote_reward", DateTime.Now);
                    return _messagesHelper.CreditsGain(reward, "TopGGVote");
                }

                return _messagesHelper.NotVotedYet();
            }

            return _messagesHelper.Error("Some problem with DB occured!");
        }
    }
}