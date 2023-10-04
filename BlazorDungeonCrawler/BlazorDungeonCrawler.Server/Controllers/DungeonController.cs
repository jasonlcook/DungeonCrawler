﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Shared.Responses;

using BlazorDungeonCrawler.Server.Data;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

namespace BlazorDungeonCrawler.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DungeonController : Controller {
        private DungeonManager dungeonManager;

        public DungeonController(DungeonManager _dungeonManager) {
            dungeonManager = _dungeonManager;
        }

        [HttpGet]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> Generate() {
            try {
                SharedDungeon dungeon = await dungeonManager.Generate();

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                //todo log errors
                return StatusCode(500);
            }
        }

        [HttpGet("{dungeonId}/tile/{tileId}")]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<TileResponse>> SelectedDungeonTile(Guid dungeonId, Guid tileId) {
            try {
                SharedTile tile = await dungeonManager.GetSelectedDungeonTile(dungeonId, tileId);

                return Ok(new TileResponse() {
                    Success = true,
                    Tile = tile
                });
            } catch (Exception ex) {
                //todo log errors
                return StatusCode(500);
            }
        }

        [HttpGet("{dungeonId}/tile/{tileId}/flee")]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> MonsterFlee(Guid dungeonId, Guid tileId) {
            try {
                SharedDungeon dungeon = await dungeonManager.MonsterFlee(dungeonId, tileId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                //todo log errors
                return StatusCode(500);
            }
        }

        [HttpGet("{dungeonId}/tile/{tileId}/fight")]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> MonsterFight(Guid dungeonId, Guid tileId) {
            try {
                SharedDungeon dungeon = await dungeonManager.MonsterFight(dungeonId, tileId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                //todo log errors
                return StatusCode(500);
            }
        }

    }
}
