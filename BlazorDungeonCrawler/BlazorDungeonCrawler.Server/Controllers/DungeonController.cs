using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Shared.Responses;

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
                Dungeon dungeon = await dungeonManager.Generate();

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
        public async Task<ActionResult<DungeonResponse>> SelectedDungeonTile(Guid dungeonId, Guid tileId) {
            try {
                Dungeon dungeon = await dungeonManager.GetSelectedDungeonTile(dungeonId, tileId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
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
                Dungeon dungeon = await dungeonManager.MonsterFlee(dungeonId, tileId);

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
                Dungeon dungeon = await dungeonManager.MonsterFight(dungeonId, tileId);

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
