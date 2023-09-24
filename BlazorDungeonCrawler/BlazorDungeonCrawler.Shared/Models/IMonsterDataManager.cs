namespace BlazorDungeonCrawler.Shared.Models {
    public interface IMonsterDataManager {
        Task<List<Monster>> GetAllMonsters();
        Task<Monster> GetMonster(int Id);
        Task<List<Monster>> SearchMonsters(string Name);
    }
}
