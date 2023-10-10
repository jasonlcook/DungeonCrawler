using SharedLevel = BlazorDungeonCrawler.Shared.Models.Level;

namespace BlazorDungeonCrawler.Server.Models {
    public class Levels {
        private readonly List<Level> _levels;

        public Levels() {
            _levels = new();
        }

        public Levels(List<SharedLevel> sharedLevels) {
            _levels = new();

            foreach (SharedLevel level in sharedLevels) {
                _levels.Add(new Level(level));
            }
        }

        public int Count() {
            return _levels.Count();
        }

        public void Add(Level level) {
            _levels.Add(level);
        }

        private List<Tile> GetTiles(int currentLevel) {
            if (_levels != null) {
                Level? level = _levels.Where(l => l.Depth == currentLevel).FirstOrDefault();
                if (level != null && level.Id != Guid.Empty) {
                    return level.Tiles;
                } else {
                    throw new Exception("Dungeon Level response was badly formed.");
                }
            } else {
                throw new Exception("Could not place tiles as Dungeon response was badly formed.");
            }
        }

        public List<SharedLevel> SharedModelMapper() {
            List<SharedLevel> sharedLevel = new();

            foreach (var level in _levels) {
                sharedLevel.Add(level.SharedModelMapper());
            }

            return sharedLevel;
        }
    }
}