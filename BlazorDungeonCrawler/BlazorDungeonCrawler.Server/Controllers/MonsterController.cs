using Microsoft.AspNetCore.Mvc;

using BlazorDungeonCrawler.Shared.Models;
using BlazorDungeonCrawler.Server.Data;

namespace BlazorDungeonCrawler.Server.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class MonsterController : Controller {
        private LocalMonsterManager monsterManager;

        public MonsterController(LocalMonsterManager _monsterManager) {
            monsterManager = _monsterManager;
        }

        [HttpGet]
        public async Task<ActionResult<MonstersResponse>> Get() {
            try {
                var Monsters = await monsterManager.GetAllMonsters();
                return Ok(new MonstersResponse() {
                    Success = true,
                    Monsters = Monsters
                });
            } catch (Exception ex) {
                return StatusCode(500);
            }
        }

        [HttpGet("{Id:int}")]
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
