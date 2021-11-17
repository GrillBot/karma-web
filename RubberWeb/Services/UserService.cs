using Discord;
using Discord.Rest;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace RubberWeb.Services
{
    public class UserService
    {
        private IMemoryCache Cache { get; }
        private DiscordRestClient DiscordClient { get; }
        private IConfiguration Configuration { get; }
        private ILogger<UserService> Logger { get; }
        private SemaphoreSlim Semaphore { get; }

        public UserService(DiscordRestClient discordRestClient, IMemoryCache cache, IConfiguration configuration,
            ILogger<UserService> logger)
        {
            DiscordClient = discordRestClient;
            Cache = cache;
            Configuration = configuration;
            Logger = logger;
            Semaphore = new SemaphoreSlim(1);

            DiscordClient.Log += DiscordClient_Log;
        }

        private Task DiscordClient_Log(LogMessage message)
        {
            var eventId = new EventId(default, message.Source);
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    Logger.LogCritical(eventId, message.Exception, message.Message);
                    break;
                case LogSeverity.Error:
                    Logger.LogError(eventId, message.Exception, message.Message);
                    break;
                case LogSeverity.Warning when message.Exception == null:
                    Logger.LogWarning(message.Message);
                    break;
                case LogSeverity.Warning when message.Exception != null:
                    Logger.LogWarning(message.Exception, message.Message);
                    break;
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                    Logger.LogInformation(message.Message);
                    break;
                case LogSeverity.Debug:
                    Logger.LogDebug(message.Message);
                    break;
            }

            return Task.CompletedTask;
        }

        public async Task<IUser> GetUserAsync(ulong userId)
        {
            await Semaphore.WaitAsync();

            try
            {
                if (Cache.TryGetValue<IUser>(userId, out var user))
                    return user;

                await InitDiscordAsync();

                user = await DiscordClient.GetUserAsync(userId);
                if (user == null) return null;

                Cache.Set(userId, user);
                return user;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private async Task InitDiscordAsync()
        {
            if (DiscordClient.LoginState == LoginState.LoggedIn) return;

            await DiscordClient.LoginAsync(TokenType.Bot, Configuration.GetConnectionString("Discord"));
        }
    }
}
