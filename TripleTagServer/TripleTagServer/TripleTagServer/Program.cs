using TripleTagServer.Hubs;
using TripleTagServer.Matchmaking;

namespace TripleTagServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();

            builder.Services.AddSingleton<MatchStore>();
            builder.Services.AddSingleton<MatchmakingService>();

            var app = builder.Build();

            app.MapGet("/", () => "TripleTagServer");

            app.MapHub<MatchHub>("/match");

            app.Run();
        }
    }
}
