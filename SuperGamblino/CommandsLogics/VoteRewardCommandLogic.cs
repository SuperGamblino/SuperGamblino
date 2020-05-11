using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public VoteRewardCommandLogic(HttpClient client, UsersConnector usersConnector, Config config,
            MessagesHelper messagesHelper, ILogger logger)
        {
            _client = client;
            _usersConnector = usersConnector;
            _config = config;
            _messagesHelper = messagesHelper;
            _logger = logger;
        }

        public async Task<DiscordEmbed> Vote(ulong userId)
        {
            const int reward = 400;
            if (_config.BotSettings.TopGgToken == null)
            {
                return _messagesHelper.Information(
                    "Top GG Token functionality is disabled on this server :disappointed:. Contact bots admin to turn it on :slight_smile:.",
                    "TopGGVote");
            }
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
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error during http request! Verify the top gg token!!!", response);
                    return _messagesHelper.Error(
                        "Invalid Top GG Token Provided by the bot administrator! Please contact him!");
                }

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