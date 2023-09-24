using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;

using BlazorDungeonCrawler.Server.Data;
using BlazorDungeonCrawler.Shared.Models;

namespace BlazorDungeonCrawler.Server.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class MonsterController : Controller {
        private LocalMonsterManager monsterManager;

        public MonsterController(LocalMonsterManager _monsterManager) {
            monsterManager = _monsterManager;
        }

        [HttpGet]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<MonstersResponse>> Get() {
            try {
                List<Monster> monsters = await monsterManager.GetAllMonsters();

                return Ok(new MonstersResponse() {
                    Success = true,
                    Monsters = monsters
                });
            } catch (Exception ex) {
                return StatusCode(500);
            }
        }

        [HttpGet("{Id:int}")]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<MonsterResponse>> Get(int Id) {
            try {
                Monster returnedMonster = await monsterManager.GetMonster(Id);

                return Ok(new MonsterResponse() {
                    Success = true,
                    Monster = returnedMonster
                });
            } catch (Exception ex) {
                return StatusCode(500);
            }
        }

        [HttpGet("{Name}/search")]
        [EnableCors("MyAllowAnyOriginMethodHeader")]
        public async Task<ActionResult<MonstersResponse>> Search(string Name) {
            try {
                List<Monster> returnedMonsters = await monsterManager.SearchMonsters(Name);

                return Ok(new MonstersResponse() {
                    Success = true,
                    Monsters = returnedMonsters
                });
            } catch (Exception ex) {
                return StatusCode(500);
            }
        }
    }
}
