using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using System.Text.Json;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Server.Database;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.AdminConsole {
    internal class AdminConsole {
        private static DungeonManager _dungeonManager;

        public static void Main(string[] args) {
            //Database context
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            var services = new ServiceCollection()
                .AddDbContextFactory<DungeonDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("BlazorDungeonCrawler")))
                .AddLogging()
                .AddLocalization();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            IDbContextFactory<DungeonDbContext> dBcontext = serviceProvider.GetService<IDbContextFactory<DungeonDbContext>>();
            if (dBcontext == null) { throw new Exception("Error creating DB context"); }


            //Logger
            ILogger<DungeonManager> logging = serviceProvider.GetService<ILogger<DungeonManager>>();
            if (logging == null) { throw new Exception("Error creating loggin"); }

            //Localiser
            var localiser = serviceProvider.GetService<IStringLocalizer<DungeonManager>>();
            if (localiser == null) { throw new Exception("Error creating localiser"); }

            //Dungeon Manager
            _dungeonManager = new(dBcontext, logging, localiser);

            try {
                MainAsync();
            } catch (Exception ex) {

                throw;
            }
        }

        private static async Task MainAsync() {
            while (true) {
                try {
                    Console.WriteLine("1. Generate Dungeon.");

                    switch (Console.ReadLine()) {
                        case "1":
                            Task<Dungeon> dungeon = _dungeonManager.Generate();
                            Console.WriteLine(JsonSerializer.Serialize(dungeon.Result));
                            break;
                        default:
                            Console.WriteLine("Unknown option");
                            break;
                    }
                } catch (Exception exp) {
                    OutputError(exp);
                }

                Console.WriteLine("Finished");
                Console.ReadLine();
            }
        }

        static void OutputError(Exception exp) {
            Console.WriteLine(exp.Message);

            if (exp.InnerException != null)
                Console.WriteLine(exp.InnerException.Message);
        }
    }
}
