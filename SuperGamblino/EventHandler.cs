using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuperGamblino
{
    class EventHandler
    {
		private DiscordClient discordClient;
		public EventHandler(DiscordClient client)
		{
			discordClient = client;
		}
		internal Task OnReady(ReadyEventArgs e)
		{
			this.discordClient.UpdateStatusAsync(new DiscordGame("Gambling House"), UserStatus.Online);
			return Task.CompletedTask;
		}
	}
}
