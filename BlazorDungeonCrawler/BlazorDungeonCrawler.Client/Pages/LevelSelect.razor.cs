using Microsoft.AspNetCore.Components;

using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class LevelSelect {
        [ParameterAttribute]
        public List<SharedFloor> DungeonFloors { get; set; }

        [ParameterAttribute]
        public bool AdventurerAlive { get; set; }

        [ParameterAttribute]
        public int DungeonDepth { get; set; }

        [Parameter]
        public EventCallback<int> OnClickCallback { get; set; }

        public LevelSelect() {
            DungeonFloors = new();
            AdventurerAlive = true;
            DungeonDepth = 0;
            OnClickCallback = new();
        }

        private async Task CallParentSelectDungeonDepthFunction(int dungeonDepth) {
            if (dungeonDepth != DungeonDepth) {
                await OnClickCallback.InvokeAsync(dungeonDepth);
            }
        }

        public string getColourClass(int dungeonDepth) {
            switch (dungeonDepth) {
                case 1:
                case 2:
                case 3:
                    return "base-colour-red";
                case 4:
                case 5:
                case 6:
                case 7:
                    return "base-colour-blue";
                case 8:
                    return "base-colour-purple";
                case 9:
                    return "base-colour-green";
                case 10:
                    return "base-colour-pink";
                default:
                    return "base-colour-unknown";
            }
        }
    }
}