using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

using BlazorDungeonCrawler.Shared.Models;

using BlazorDungeonCrawler.Server.Database;
using BlazorDungeonCrawler.Server.Database.Resources.Commands.Create;
using BlazorDungeonCrawler.Server.Database.Resources.Queries.Get;

namespace BlazorDungeonCrawler.Server.Data {
    public class AccessTokenManager {
        private readonly ILogger _logger;                                       //  Logger.               Errors and important information is returned to Azure application insights and the debug console.
        private readonly IStringLocalizer<DungeonManager> _localiser;           //  Localiser.            All messages returned to the user from the localiser via the DungeonManager resource file. 
        private readonly IDbContextFactory<DungeonDbContext> _contextFactory;   //	Database Context.     Reference to the database context.

        private readonly MessageManager _messageManager;

        //***********************************************************
        //*********************************************** Constructor
        public AccessTokenManager(IDbContextFactory<DungeonDbContext> contextFactory, ILogger<DungeonManager> logger, IStringLocalizer<DungeonManager> localiser) {
            this._localiser = localiser;
            this._messageManager = new MessageManager(_localiser);

            this._contextFactory = contextFactory;
            this._logger = logger;

            _logger.LogInformation("Token manager initiated. {DT}", DateTime.UtcNow.ToLongTimeString());
        }

        //***********************************************************
        //************************************************** Generate

        public async Task<AccessToken> Generate(string name) {
            if (name == string.Empty) { throw new ArgumentNullException("Token name"); }

            string token = Guid.NewGuid().ToString();
            AccessToken accessToken = new(token, name);

            AccessTokenCreate accessTokenCreate = new(_contextFactory.CreateDbContext(), _logger);
            await accessTokenCreate.Create(accessToken);

            _logger.LogInformation($"Access token {accessToken.Id} generated.");

            return accessToken;
        }

        //***********************************************************
        //*************************************** RetrieveAccessToken

        public async Task<AccessToken> RetrieveAccessToken(string token) {
            if (token == string.Empty) { throw new ArgumentNullException("Token"); }

            _logger.LogInformation($"Token {token} retrieve.");

            AccessTokenQueries accessTokenQueries = new(_contextFactory.CreateDbContext(), _logger);
            AccessToken? accessToken = await accessTokenQueries.Get(token);

            if (accessToken == null || accessToken.Id == Guid.Empty) {
                throw new Exception(_messageManager.ErrorTokenNoFound());
            }

            return accessToken;
        }
    }
}
