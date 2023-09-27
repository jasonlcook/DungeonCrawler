using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class DungeonController : Controller {
        private DungeonManager dungeonManager;

        public DungeonController(DungeonManager _dungeonManager) {
            dungeonManager = _dungeonManager;
        }

        [HttpGet]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> Get() {
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
        public async Task<ActionResult<DungeonResponse>> Get(Guid dungeonId, Guid tileId) {
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

    }
}
