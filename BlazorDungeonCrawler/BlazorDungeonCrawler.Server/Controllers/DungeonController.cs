using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Shared.Responses;

using BlazorDungeonCrawler.Server.Data;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;
using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;

namespace BlazorDungeonCrawler.Server.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class DungeonController : Controller {
        private DungeonManager dungeonManager;

        public DungeonController(DungeonManager _dungeonManager) {
            dungeonManager = _dungeonManager;
        }

        [HttpGet]
        [EnableCors("AllowAnyOriginMethodHeader")]
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

        [HttpGet("{dungeonId}")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> RetrieveDungeon(Guid dungeonId) {
            try {
                SharedDungeon dungeon = await dungeonManager.RetrieveDungeon(dungeonId);

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
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<TilesResponse>> SelectedDungeonTile(Guid dungeonId, Guid tileId) {
            try {
                List<SharedTile> tiles = await dungeonManager.GetSelectedDungeonTiles(dungeonId, tileId);

                return Ok(new TilesResponse() {
                    Success = true,
                    Tiles = tiles
                });
            } catch (Exception ex) {
                //todo log errors
                return StatusCode(500);
            }
        }

        [HttpGet("{dungeonId}/tile/{tileId}/flee")]
        [EnableCors("AllowAnyOriginMethodHeader")]
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
        [EnableCors("AllowAnyOriginMethodHeader")]
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
