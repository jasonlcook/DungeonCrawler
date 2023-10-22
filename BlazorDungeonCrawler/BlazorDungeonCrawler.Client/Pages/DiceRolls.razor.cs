using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace BlazorDungeonCrawler.Client.Pages {
    public class Die {
        public string Face { get; set; }
        public string Type { get; set; }
        public int Rotation { get; set; }

        public Die(string face, string type, int rotation) {
            Face = face;
            Type = type;
            Rotation = rotation;
        }
    }

    public partial class DiceRolls {
        [Parameter]
        public string SafeDice { get; set; }

        [Parameter]
        public string DangerDice { get; set; }

        public DiceRolls() {
            SafeDice = string.Empty;
            DangerDice = string.Empty;
        }

        public List<Die> parseDice(string rawDice, string type) {
            List<Die> dice = new();
            string face = string.Empty;
            foreach (string die in rawDice.Split(',')) {
                face = "empty";

                switch (die) {
                    case "1":
                        face = "one";
                        break;
                    case "2":
                        face = "two";
                        break;
                    case "3":
                        face = "three";
                        break;
                    case "4":
                        face = "four";
                        break;
                    case "5":
                        face = "five";
                        break;
                    case "6":
                        face = "six";
                        break;
                }

                dice.Add(new Die(face, type, 0));
            }

            return dice;
        }
    }
}