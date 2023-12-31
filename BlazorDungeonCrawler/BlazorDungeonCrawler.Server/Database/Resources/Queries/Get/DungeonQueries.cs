﻿using Microsoft.EntityFrameworkCore;

using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Database.Resources.Queries.Get {
    public class DungeonQueries {
        protected readonly DungeonDbContext _dbContext;
        private readonly ILogger _logger;

        public DungeonQueries(DungeonDbContext dbContext, ILogger logger) {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Dungeon?> Get(Guid dungeonId) {
            if (dungeonId == Guid.Empty) { throw new ArgumentNullException("Dungeon query get without Id"); }

            try {
                _logger.LogInformation("Retrieving Dungeon");

                return await _dbContext.Dungeons
                    .Include("Adventurer")
                    .Include(d => d.Floors.Where(f => f.IsCurrent == true))
                    .Include("Floors.Tiles")
                    .Include("Floors.Tiles.Monsters")
                    .Include(d => d.Messages.Where(m => m.DungeonId == dungeonId).OrderByDescending(m => m.Datestamp).Take(10))
                    .Include("Messages.Children")
                    .Include("Messages.Children.Children")
                    .FirstAsync(d => d.Id == dungeonId);
            } catch (Exception ex) {
                _logger.LogError($"DungeonQueries Get Error: {ex.Message}");
                throw new Exception("Database error while attempting to retrieve the Dungeon.");
            }
        }
    }
}
