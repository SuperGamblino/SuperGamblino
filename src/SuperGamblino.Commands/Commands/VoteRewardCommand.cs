using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SuperGamblino.Core.Configuration;
using SuperGamblino.Core.GamesObjects;
using SuperGamblino.Infrastructure.Connectors;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class VoteRewardCommand
    {
        private const int Reward = 400;
        private readonly HttpClient _client;
        private readonly Config _config;
        private readonly ILogger<VoteRewardCommand> _logger;
        private readonly MessagesHelper _messagesHelper;
        private readonly UsersConnector _usersConnector;

        public VoteRewardCommand(HttpClient client, UsersConnector usersConnector, Config config,
            MessagesHelper messagesHelper, ILogger<VoteRewardCommand> logger)
        {
            _client = client;
            _usersConnector = usersConnector;
            _config = config;
            _messagesHelper = messagesHelper;
            _logger = logger;
        }

        public async Task<Message> Vote(ulong userId)
        {
            if (_config.BotSettings.TopGgToken == null)
                return _messagesHelper.Information(
                    "Top GG Token functionality is disabled on this server :disappointed:. Contact bots admin to turn it on :slight_smile:.",
                    "TopGGVote");

            var user = await _usersConnector.GetUser(userId);
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

            if (didUserVote.Voted)
            {
                if (user.LastVoteReward.HasValue)
                {
                    var timeSpan = DateTime.Now - user.LastVoteReward.Value;
                    if (timeSpan >= TimeSpan.FromHours(12))
                    {
                        user.Credits += Reward;
                        user.LastVoteReward = DateTime.Now;
                        await _usersConnector.UpdateUser(user);
                        return _messagesHelper.CreditsGain(Reward, "TopGGVote");
                    }

                    return _messagesHelper.CommandCalledTooEarly(TimeSpan.FromHours(12) - timeSpan, "!vote",
                        "TopGGVote");
                }

                user.Credits += Reward;
                user.LastVoteReward = DateTime.Now;
                await _usersConnector.UpdateUser(user);
                return _messagesHelper.CreditsGain(Reward, "TopGGVote");
            }

            return _messagesHelper.NotVotedYet();
        }
    }
}