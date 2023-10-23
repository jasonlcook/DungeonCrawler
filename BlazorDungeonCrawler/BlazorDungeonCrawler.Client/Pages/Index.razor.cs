using Microsoft.JSInterop;

using BlazorDungeonCrawler.Shared.Models;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using SharedFloor = BlazorDungeonCrawler.Shared.Models.Floor;
using System.Runtime.CompilerServices;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class Index {
        //Page elements
        public bool? FoundCookie { get; set; }
        public bool RejectedCookie { get; set; }

        public List<string> ErrorReports { get; set; }
        public List<string> InformationReports { get; set; }

        public bool AdvanceDisabled { get; set; }

        public Guid DungeonId { get; set; }
        public int DungeonDepth { get; set; }

        public bool MacGuffinFound { get; set; }

        public string ApiVersion { get; set; }

        List<SharedTile> DungeonTiles { get; set; }

        public bool GameOver { get; set; }

        public List<SharedFloor> DungeonFloors { get; set; }

        public List<Attribute> AdventurerExperienceStats { get; set; }
        public List<Attribute> AdventurerHealthStats { get; set; }
        public List<Attribute> AdventurerDamageStats { get; set; }
        public List<Attribute> AdventurerProtectionStats { get; set; }
        public List<Message> Messages { get; set; }

        public string SafeDice { get; set; }
        public string DangerDice { get; set; }

        public Index() {
            FoundCookie = null;
            RejectedCookie = false;

            ErrorReports = new();
            InformationReports = new();

            AdvanceDisabled = true;

            DungeonId = Guid.Empty;
            DungeonDepth = 0;

            MacGuffinFound = false;

            ApiVersion = "API V0.0.0";

            DungeonTiles = new();

            GameOver = false;
            DungeonFloors = new();

            AdventurerExperienceStats = new();
            AdventurerHealthStats = new();
            AdventurerDamageStats = new();
            AdventurerProtectionStats = new();
            Messages = new();

            SafeDice = string.Empty;
            DangerDice = string.Empty;
        }

        //Cookies
        string cookieKeyId = "BlazorWebAppCookies-Id";

        Guid cookieId = Guid.Empty;

        //Dungeon
        Dungeon dungeon = new();
        Floor floor = new();
        Adventurer adventurer = new();

        public async Task WriteLog(string message) {
            await this.JS.InvokeVoidAsync("console.log", message);
        }

        //Cookies
        protected override void OnInitialized() {
            if (FoundCookie == null) {
                CheckCookies();
            }
        }

        //  Create
        private async void StoreCookie(Guid dungeonId) {
            string safeDungeonId = dungeonId.ToString();
            string cookie = BakeCookie(cookieKeyId, safeDungeonId, 7);
            await SetCookie(cookie);
        }

        private string BakeCookie(string key, string value, double days) {
            string dateStamp = "";
            if (days > 0) {
                dateStamp = DateTime.Now.AddDays(days).ToUniversalTime().ToString("R");
            }

            return $"{key}={value}; expires={dateStamp}; path=/";
        }

        private async Task SetCookie(string cookie) {
            await JS.InvokeVoidAsync("eval", $"document.cookie = \"{cookie}\"");
        }

        //  Retrieve
        private async void CheckCookies() {
            string response = GetCookie();
            Dictionary<string, string> cookies = ParseCookieResponse(response);

            if (cookies.ContainsKey(cookieKeyId)) {
                FoundCookie = true;

                if (Guid.TryParse(cookies[cookieKeyId], out cookieId)) {
                    await WriteLog($"COOKIEID: {cookieId}");
                }
            } else {
                FoundCookie = false;
            }
        }

        private string GetCookie() {
            return ((IJSInProcessRuntime)JS).Invoke<string>("eval", $"document.cookie");
        }

        private Dictionary<string, string> ParseCookieResponse(string value) {
            Dictionary<string, string> cookies = new();

            if (!string.IsNullOrEmpty(value)) {
                string[] values = value.Split(';');

                string cookieKey, cookieValue;
                foreach (string val in values) {
                    cookieKey = val.Substring(0, val.IndexOf('=')).Trim();
                    cookieValue = val.Substring(val.IndexOf('=') + 1);
                    cookies.Add(cookieKey, cookieValue);
                }
            }

            return cookies;
        }

        //Dungeon
        protected override async void OnAfterRender(bool firstRender) {
            ErrorReports = new();
            InformationReports = new();

            try {
                if (dungeon == null || dungeon.Id == Guid.Empty) {
                    Dungeon? dungeon;
                    try {
                        if (cookieId != Guid.Empty) {
                            dungeon = await DungeonManager.GetDungeon(cookieId);
                        } else {
                            dungeon = await DungeonManager.GenerateNewDungeon();
                        }

                        ValidateDungeon(dungeon);
                    } catch (Exception ex) {
                        ErrorReports.Add(ex.Message);
                    }
                    if (!await UpdatePageVariables()) {
                        InformationReports.Add("Dungeon values could not udpdated.");
                    }
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        public void SelectDungeonDepth(int dungeonDepth) {
            if (dungeonDepth == 0) { throw new Exception("Dungeon depth element was badly formed."); }
            if (dungeon == null || dungeon.Id == Guid.Empty) { throw new Exception("Dungeon element was badly formed."); }

            try {
                dungeon.Depth = dungeonDepth;
                ValidateDungeon(dungeon);
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        public async Task SelectTile(Guid tileId) {
            if (tileId == Guid.Empty) { throw new Exception("Tile element was badly formed."); }
            if (dungeon == null || dungeon.Id == Guid.Empty) { throw new Exception("Dungeon element was badly formed."); }

            ErrorReports = new();
            InformationReports = new();

            try {
                ValidateDungeon(await DungeonManager.SelectDungeonTile(dungeon.Id, tileId));
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }

            if (!await UpdatePageVariables()) { InformationReports.Add("Dungeon values could not udpdated."); }
        }

        private async Task AutomaticallyAdvanceDungeon() {
            if (dungeon == null || dungeon.Id == Guid.Empty) { throw new Exception("Dungeon element was badly formed."); }

            try {
                AdvanceDisabled = true;

                try {
                    ValidateDungeon(await DungeonManager.AutomaticallyAdvanceDungeon(dungeon.Id));
                } catch (Exception ex) {
                    ErrorReports.Add(ex.Message);
                }

                if (!await UpdatePageVariables()) {
                    InformationReports.Add("Dungeon values could not udpdated.");
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private async Task MonsterFight() {
            if (dungeon == null || dungeon.Id == Guid.Empty) { throw new Exception("Dungeon element was badly formed."); }

            try {
                try {
                    ValidateDungeon(await DungeonManager.MonsterFight(dungeon.Id, dungeon.CombatTile));
                } catch (Exception ex) {
                    ErrorReports.Add(ex.Message);
                }

                if (!await UpdatePageVariables()) {
                    InformationReports.Add("Dungeon values could not udpdated.");
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private async Task MonsterFlee() {
            if (dungeon == null || dungeon.Id == Guid.Empty) { throw new Exception("Dungeon element was badly formed."); }

            try {
                try {
                    ValidateDungeon(await DungeonManager.MonsterFlee(dungeon.Id, dungeon.CombatTile));
                } catch (Exception ex) {
                    ErrorReports.Add(ex.Message);
                }

                if (!await UpdatePageVariables()) {
                    InformationReports.Add("Dungeon values could not udpdated.");
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private void RemainOnFloor() {
            dungeon.StairsDiscovered = false;
        }

        private async Task DescendStairs() {
            if (dungeon == null || dungeon.Id == Guid.Empty) { throw new Exception("Dungeon element was badly formed."); }

            try {
                try {
                    ValidateDungeon(await DungeonManager.DescendStairs(dungeon.Id));
                } catch (Exception ex) {
                    ErrorReports.Add(ex.Message);
                }

                if (!await UpdatePageVariables()) {
                    InformationReports.Add("Dungeon values could not udpdated.");
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        public async Task UpdateDice((string safeDice, string dangerDice) args) {
            SafeDice = args.safeDice;
            DangerDice = args.dangerDice;

            await InvokeAsync(StateHasChanged);
        }

        public async Task NewGame() {
            try {
                Dungeon? dungeon = await DungeonManager.GenerateNewDungeon();
                ValidateDungeon(dungeon);

                if (!await UpdatePageVariables()) {
                    InformationReports.Add("Dungeon values could not udpdated.");
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private void ValidateDungeon(Dungeon _dungeon) {
            try {
                //Check and assign Dungeon
                if (_dungeon == null || _dungeon.Id == Guid.Empty) { throw new ArgumentNullException("Dungon"); };
                dungeon = _dungeon;

                //If cookie has been set then refresh cookie with new dungeon Id
                if (FoundCookie == true && cookieId != dungeon.Id) {
                    StoreCookie(dungeon.Id);
                }

                DungeonId = dungeon.Id;
                DungeonDepth = dungeon.Depth;

                MacGuffinFound = dungeon.MacGuffinFound;

                GameOver = dungeon.GameOver;

                if (dungeon.GameOver) {
                    AdvanceDisabled = true;
                }

                //Check and assign Dungeon Floor
                if (_dungeon.Floors == null || _dungeon.Floors.Count == 0) { throw new ArgumentNullException("Dungon Floors"); };
                DungeonFloors = _dungeon.Floors.OrderBy(f => f.Depth).ToList();

                Floor? _floor = _dungeon.Floors.Where(l => l.Depth == DungeonDepth).FirstOrDefault();
                if (_floor == null || _floor.Id == Guid.Empty) { throw new ArgumentNullException("Dungon Floor"); };
                floor = _floor;

                if (_floor.Tiles == null || _floor.Tiles.Count == 0) { throw new ArgumentNullException("Dungon Floors"); };
                List<SharedTile>? _tiles = _floor.Tiles;
                if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungon Floor Tiles"); };
                DungeonTiles = _tiles;

                //Check and assign Dungeon Floor
                if (_dungeon.Adventurer == null || _dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
                adventurer = _dungeon.Adventurer;

                //Check and assign Dungeon Messages
                if (_dungeon.Messages == null || _dungeon.Messages.Count == 0) { throw new ArgumentNullException("Dungeon Messages"); }
                Messages = _dungeon.Messages.OrderByDescending(m => m.Datestamp).ToList();
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private async Task<bool> UpdatePageVariables() {
            try {
                AdventurerExperienceStats = new();
                AdventurerHealthStats = new();
                AdventurerDamageStats = new();
                AdventurerProtectionStats = new();

                //Advance button
                //  Enable
                AdvanceDisabled = false;

                //  Stats
                //      XP
                AdventurerExperienceStats.Add(new Attribute() {
                    Label = "Level",
                    Value = adventurer.ExperienceLevel.ToString()
                });

                AdventurerExperienceStats.Add(new Attribute() {
                    Label = "Experience",
                    Value = $"{adventurer.Experience} / {adventurer.NextExperienceLevelCost}"
                });

                //      Health
                int auraPotion = adventurer.AuraPotion;
                int auraPotionDuration = adventurer.AuraPotionDuration;

                if (auraPotionDuration > 0) {
                    AdventurerHealthStats.Add(new Attribute() {
                        Label = "Aura potion",
                        Value = auraPotion.ToString(),
                        Duration = auraPotionDuration.ToString()
                    });

                    AdventurerHealthStats.Add(new Attribute() {
                        Label = "Base",
                        Value = adventurer.HealthBase.ToString()
                    });
                }

                AdventurerHealthStats.Add(new Attribute() {
                    Label = "Health",
                    Value = $"{adventurer.HealthBase + auraPotionDuration} / {adventurer.HealthInitial}"
                });

                //      Damage
                //          Potion
                int damagePotion = adventurer.DamagePotion;
                int damagePotionDuration = adventurer.DamagePotionDuration;
                if (damagePotionDuration > 0) {
                    AdventurerDamageStats.Add(new Attribute() {
                        Label = "Damage potion",
                        Value = damagePotion.ToString(),
                        Duration = damagePotionDuration.ToString()
                    });
                }

                //          Weapon
                int weapon = adventurer.Weapon;
                if (weapon > 0) {
                    AdventurerDamageStats.Add(new Attribute() {
                        Label = "Weapon",
                        Value = weapon.ToString()
                    });
                }

                if (damagePotionDuration > 0 || weapon > 0) {
                    AdventurerDamageStats.Add(new Attribute() {
                        Label = "Base",
                        Value = adventurer.DamageBase.ToString()
                    });
                }

                AdventurerDamageStats.Add(new Attribute() {
                    Label = "Damage",
                    Value = (adventurer.DamageBase + damagePotion + weapon).ToString()
                });

                //      Protection
                //          Potion
                int shieldPotion = adventurer.ShieldPotion;
                int shieldPotionDuration = adventurer.ShieldPotionDuration;
                if (shieldPotionDuration > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Shield potion",
                        Value = shieldPotion.ToString(),
                        Duration = shieldPotionDuration.ToString()
                    });
                }

                //          Armour
                //              Helmet
                int armourHelmet = adventurer.ArmourHelmet;
                if (armourHelmet > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Helmet",
                        Value = armourHelmet.ToString()
                    });
                }

                //              Breastplate
                int armourBreastplate = adventurer.ArmourBreastplate;
                if (armourBreastplate > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Breastplate",
                        Value = armourBreastplate.ToString()
                    });
                }

                //              Gauntlet
                int armourGauntlet = adventurer.ArmourGauntlet;
                if (armourGauntlet > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Gauntlet",
                        Value = armourGauntlet.ToString()
                    });
                }

                //              Greave
                int armourGreave = adventurer.ArmourGreave;
                if (armourGreave > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Greave",
                        Value = armourGreave.ToString()
                    });
                }

                //              Boots
                int armourBoots = adventurer.ArmourBoots;
                if (armourBoots > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Boots",
                        Value = armourBoots.ToString()
                    });
                }

                int protectionAdditions = shieldPotion + armourHelmet + armourBreastplate + armourGauntlet + armourGreave + armourBoots;
                if (protectionAdditions > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Base",
                        Value = adventurer.ProtectionBase.ToString()
                    });
                }

                int protection = adventurer.ProtectionBase + protectionAdditions;
                if (protection > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Protection",
                        Value = protection.ToString()
                    });
                }

                //API version
                ApiVersion = $"API V{dungeon.ApiVersion}";

                await InvokeAsync(StateHasChanged);
            } catch (Exception) {
                throw;
            }

            return true;
        }
    }
}