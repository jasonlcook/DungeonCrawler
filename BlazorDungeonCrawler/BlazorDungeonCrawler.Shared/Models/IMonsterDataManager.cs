using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorDungeonCrawler.Shared.Models {
    public interface IMonsterDataManager {
        Task<List<Monster>> GetAllMonsters();
        Task<Monster> GetMonster(int Id);
        Task<List<Monster>> SearchMonsters(string Name);
    }
}
