using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SuperGamblino.Messages;

namespace SuperGamblino.Commands.Commands
{
    public class CountDEBUGCommand
    {
        private ILogger<CountDEBUGCommand> _logger;

        public CountDEBUGCommand(ILogger<CountDEBUGCommand> logger)
        {
            _logger = logger;
        }

        public Task Count(ref Message message)
        {
            message.Description = "Counting...";
            Task.Delay(5_000);
            for (int i = 0; i < 10; i++)
            {
                message.Description = i.ToString();
                message.Update();
                _logger.LogInformation("updated");
                Thread.Sleep(2_000);
            }
            message.Delete();
            return Task.CompletedTask;
        }
    }
}