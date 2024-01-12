//**********************************************************************************************************************
//  API controller 

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Shared.Responses;
using BlazorDungeonCrawler.Server.Data;

using SharedDungeon = BlazorDungeonCrawler.Shared.Models.Dungeon;

namespace BlazorDungeonCrawler.Server.Controllers {

    [ApiController]
    [Route("api/dungeon")]
    public class DungeonController : Controller {
        private ILogger<DungeonController> _logger;
        private DungeonManager _dungeonManager;

        public DungeonController(DungeonManager dungeonManager, ILogger<DungeonController> logger) {
            this._dungeonManager = dungeonManager;
            this._logger = logger;

        }

        [HttpGet]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> Generate() {
            try {
                SharedDungeon dungeon = await _dungeonManager.Generate();

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Generate Dungeon Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }

        [HttpGet("{dungeonId}")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> RetrieveDungeon(Guid dungeonId) {
            try {
                SharedDungeon dungeon = await _dungeonManager.RetrieveDungeon(dungeonId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Retrieve Dungeon Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }

        [HttpGet("{dungeonId}/tile/{tileId}")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> SelectedDungeonTile(Guid dungeonId, Guid tileId) {
            try {
                SharedDungeon dungeon = await _dungeonManager.GetSelectedDungeonTiles(dungeonId, tileId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Selected Dungeon tile Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }

        [HttpGet("{dungeonId}/automaticallyadvancedungeon")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> AutomaticallyAdvanceDungeon(Guid dungeonId) {
            try {
                SharedDungeon dungeon = await _dungeonManager.AutomaticallyAdvanceDungeon(dungeonId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Automatically advance Dungeon Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }

        [HttpGet("{dungeonId}/descendstairs")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> DescendStairs(Guid dungeonId) {
            try {
                SharedDungeon dungeon = await _dungeonManager.DescendStairs(dungeonId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Descend Dungeon stairs Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }


        [HttpGet("{dungeonId}/tile/{tileId}/flee")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> MonsterFlee(Guid dungeonId, Guid tileId) {
            try {
                SharedDungeon dungeon = await _dungeonManager.MonsterFlee(dungeonId, tileId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Monster flee Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }

        [HttpGet("{dungeonId}/tile/{tileId}/fight")]
        [EnableCors("AllowAnyOriginMethodHeader")]
        public async Task<ActionResult<DungeonResponse>> MonsterFight(Guid dungeonId, Guid tileId) {
            try {
                SharedDungeon dungeon = await _dungeonManager.MonsterFight(dungeonId, tileId);

                return Ok(new DungeonResponse() {
                    Success = true,
                    Dungeon = dungeon
                });
            } catch (Exception ex) {
                _logger.LogError($"Monster fight Error: {ex.Message}");

                return StatusCode(500, new DungeonResponse() {
                    Success = false,
                    Dungeon = new(),
                    ErrorMessages = ex.Message
                });
            }
        }

    }
}
